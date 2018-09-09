using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using AzureDevOps.Exception.Reporter;
using Kongsberg.Nemo.ExceptionReporter.Plugin.UI;

namespace AzureDevOps.ExceptionReporter.Km
{
    public class ReportForm 
    {
        /// <summary>
        /// objects used for sync locking. 
        /// </summary>
        private object syncRoot = new object();

        public void RegisterExceptionEvents(Action<System.Exception, bool> callback, bool tryContinueAfterException)
        {
            //How to handle unhandled excpetions in WPF:
            //see: http://msdn.microsoft.com/en-us/library/system.windows.application.dispatcherunhandledexception.aspx
            //but unhandled 
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.DispatcherUnhandledException +=
                    (sender, args) =>
                    {
                        lock (this.syncRoot)
                        {
                            //True: application will continue.
                            //False: default unhandled exception processing
                            callback(args.Exception, !args.Handled);
                            args.Handled = CanBeSafelySkipped(args) || tryContinueAfterException;
                        }
                    };
            }
            //  LARS: do not do this for KM: WPF and WinForm in same.          else
            //              throw new NullReferenceException("Failed to register unhandled excpetions delegate, no exceptions will be trapped. Reason:  Application.Current does not exists.");


            try
            {

                /*    SINCE WE DO NOT HAVE ANY FALLBACK ALL EXCEPTIONS WILL RESULT IN AppDomain.CurrentDomain.UnhandledException 
                 *  otherwise winforms applications will report same exception twice...
                 *
                 * System.Windows.Forms.Application.ThreadException += (sender, args) =>
                                                                            {
                                                                                lock (this.syncRoot)
                                                                                {
                                                                                    callback(args.Exception, true);
                                                                                }
                                                                            };
                    */
                //catch exceptions.
                try
                {
                    System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                }
                catch
                {
                }

                //since WPF DispatcherUnhandledException do not hook on child thread, register with Appdomain unhandled Exceptions register with Appdomain 
                AppDomain.CurrentDomain.UnhandledException +=
                    (sender, args) =>
                    {
                        lock (this.syncRoot)
                        {
                            callback(args.ExceptionObject as System.Exception, args.IsTerminating);
                            
                        }
                    };
                // some apps has it's own app domain exception registator, and the only way to check it is "try"
            }
            catch
            {
            }
        }

        /// <summary>
        /// Supresses some exceptions temporary.
        /// Rarely on close we have "Exception message: Dispatcher processing has been suspended, but messages are still being processed.Type: System.InvalidOperationException"
        /// which is not important and can be safely skipped. It is unclear why we have it for now.
        /// </summary>
        private static bool CanBeSafelySkipped(DispatcherUnhandledExceptionEventArgs args)
        {
            return args.Exception is InvalidOperationException &&
                args.Exception.Message.Contains("Dispatcher processing has been suspended, but messages are still being processed.");
        }

        public ReportFormUI Window { get; set; }

        public bool ShowException(string errorText, ReportException report)
        {
            bool posted = false;
            Window = new ReportFormUI();
            Window.txtError.Text = errorText;
            Window.btnPost.Click +=
                (sender, args) =>
                {
                    try
                    {
                        var text = Window.txtDescription == null ? "" : Window.txtDescription.Text;
                        report(text);
                        posted = true;
                        Window.Close();
                    }
                    catch (System.Exception)
                    {
                        //failure 
                        //Nothing relevant to report.
                    }
                };
            Window.Topmost = true;
            Window.ShowDialog();

            Window = null;
            return posted;
        }

        public void ShowDeliveryFailure(string message, System.Exception deliveryException)
        {
            System.Windows.MessageBox.Show(message, "Delivery failure", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
