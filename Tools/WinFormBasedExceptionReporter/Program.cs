using System;
using System.Configuration;
using System.Windows.Forms;
using AzureDevOps.Exception.Reporter;
using AzureDevOps.Exception.Service.Common;

namespace ExceptionReporterTestApp
{
    static class Program
    {
        internal static ExceptionRegistrator ExceptionRegistrator;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            var useGui = bool.Parse(ConfigurationManager.AppSettings["ExceptionReporterUseGUI"]);

            try
            {
                // try to initialize granular plugin implementation
                ExceptionRegistrator = new ExceptionRegistrator("WinFormExceptionReport", useGui,
                new ServiceSettings(new Uri(ConfigurationManager.AppSettings["serviceURL"]), "", ""));
            }
            catch (ArgumentException)
            {
                // ok, wrong plugin, try another one
                ExceptionRegistrator = new ExceptionRegistrator(
                new ExceptionHandlerSettings(
                    "KmTestWinFrom", true, true, ConfigurationManager.AppSettings["serviceURL"])
                );
            }
                
            Application.Run(new Form1());
        }
    }
}
