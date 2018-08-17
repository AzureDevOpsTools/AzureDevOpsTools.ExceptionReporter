using System;
using System.Windows;
using Inmeta.Exception.Reporter;
using Inmeta.Exception.Service.Common;
using System.Configuration;

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
            var uri = StartupUri;

            
                // try to initialize granular plugin implementation
            Startup += (sender, args) =>
                           {
                               try
                               {
                                   StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
                                   ExceptionRegistrator = new ExceptionRegistrator("WPFBasedExceptionReporter", true,
                                                                                   new DefaultServiceSettings());
                               }
                               catch (ArgumentException)
                               {
                                   StartupUri = new Uri("KmMainWindow.xaml", UriKind.Relative);
                                   ExceptionRegistrator = new ExceptionRegistrator(
                                       new ExceptionHandlerSettings(
                                           ConfigurationManager.AppSettings["ApplicationName"],
                                           bool.Parse(ConfigurationManager.AppSettings["TryContinueAfterException"]),
                                           bool.Parse(ConfigurationManager.AppSettings["ShowExitWindow"]),
                                           ConfigurationManager.AppSettings["serviceURL"],
                                           bool.Parse(ConfigurationManager.AppSettings["LogExceptionReports"]),
                                           bool.Parse(ConfigurationManager.AppSettings["ExceptionReporterUseGUI"])));

                               }
                           };

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = bool.Parse(ConfigurationManager.AppSettings["TryContinueAfterException"]);
        }
    }
}
