using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Inmeta.Exception.Reporter;

namespace Inmeta.Exception.ReportUI.WPF
{
    [Export(typeof(IExceptionReportView))]
    public class WPFExceptionReporterView : IExceptionReportView
    {

        public void ShowException(string errorText, ReportException post, ReportException cancel)
        {
            //fix MTA /STA thread issue in WPF
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<string, ReportException, ReportException>(ShowException), errorText, post, cancel);

            var window = new ReportFormUI {txtError = {Text = errorText}, Topmost =  true};

            //register post event 
            window.btnPost.Click += (sender, args) =>
                                        {
                                            post(window.txtDescription.Text);
                                            //in WPF isDefault btn must manually be set and closed
                                            window.DialogResult = true;
                                            window.Close();
                                        };

            //register cancel event
            window.btnCancel.Click += (sender, args) =>cancel(window.txtDescription.Text);

            window.ShowDialog();
        }

        public void ShowDeliveryFailure(string message)
        {
            //Fix MTA/STA issues
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<string>(ShowDeliveryFailure), message);
            
            MessageBox.Show(message, "Delivery failure", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowTerminateDialog()
        {
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action(ShowTerminateDialog));

            var terminateWindow = new TerminatingWindow {Topmost = true};
            terminateWindow.Show();

            //force refresh trick for WPF : invoke empty delgate to force refresh
            //see: http://geekswithblogs.net/NewThingsILearned/archive/2008/08/25/refresh--update-wpf-controls.aspx
            terminateWindow.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));

            //redraw
            terminateWindow.InvalidateVisual();

            //sleep for 5000 seconds.
            Thread.Sleep(5000);

            terminateWindow.Close();

        }
    }
}