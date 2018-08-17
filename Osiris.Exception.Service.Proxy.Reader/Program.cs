using System.ServiceProcess;

namespace Inmeta.Exception.Service.Proxy.Reader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ExceptionReaderService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
