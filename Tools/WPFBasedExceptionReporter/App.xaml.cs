using System.Windows;
using Inmeta.Exception.Reporter;
using Inmeta.Exception.Service.Common;

namespace WPFBasedExceptionReporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ExceptionRegistrator ExceptionRegistrator { get; set;}
   
        public App()
        {
            Startup += (sender, args) => ExceptionRegistrator = new ExceptionRegistrator("WPFBasedExceptionReporter", true, new DefaultServiceSettings());
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
