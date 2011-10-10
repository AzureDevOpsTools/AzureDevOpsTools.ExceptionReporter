using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Inmeta.Exception.Reporter;

namespace Inmeta.Exception.Report.WinForm 
{
    [Export (typeof(IExceptionTrappingStrategy))]
    public class WinFormExceptionReporterTrappingStrategy : IExceptionTrappingStrategy
    {        
        /// <summary>
        /// objects used for sync locking. 
        /// </summary>
        private object syncRoot = new object();

        public void RegisterExceptionEvents(Action<System.Exception, bool> callback)
        {

            Application.ThreadException += (sender, args) =>
            {
                lock (syncRoot)
                {
                    callback(args.Exception, false);
                }
            };
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            }
            catch
            {
                //this is ok.
            }

            //since WPF DispatcherUnhandledException do not hook on child thread, register with Appdomain unhandled Exceptions register with Appdomain 
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                lock (syncRoot)
                {
                    callback(
                        args.ExceptionObject as System.Exception,
                        args.IsTerminating);
                }
            };
        }


        public void UnRegister()
        {
            //nothing to do unregister.
        }
    }
}
