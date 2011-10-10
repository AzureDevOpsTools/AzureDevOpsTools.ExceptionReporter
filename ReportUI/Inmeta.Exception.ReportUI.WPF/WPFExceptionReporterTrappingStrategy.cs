using System;
using System.ComponentModel.Composition;
using System.Windows;
using Inmeta.Exception.Reporter;

namespace Inmeta.Exception.ReportUI.WPF
{
    [Export(typeof(IExceptionTrappingStrategy))]
    public class WPFExceptionReporterTrappingStrategy : IExceptionTrappingStrategy
    {
        public void RegisterExceptionEvents(Action<System.Exception, bool> callback)
        {
            //How to handle unhandled excpetions in WPF:
            //see: http://msdn.microsoft.com/en-us/library/system.windows.application.dispatcherunhandledexception.aspx
            if (Application.Current != null)
                Application.Current.DispatcherUnhandledException += (sender, args) => callback(args.Exception, !args.Handled);
            else
                throw new NullReferenceException("Failed to register unhandled excpetions delegate, no exceptions will be trapped. Reason:  Application.Current does not exists.");

            //since WPF DispatcherUnhandledException do not hook on child thread, register with Appdomain unhandled Exceptions register with Appdomain 
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => callback(args.ExceptionObject as System.Exception, args.IsTerminating);


        }

        public void UnRegister()
        {
            //nothing to do
        }
    
    }
}
