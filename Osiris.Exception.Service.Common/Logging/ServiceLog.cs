using log4net;

namespace Inmeta.Exception.Common
{

	public class ServiceLog
	{
		private static ILog log;

		public static ILog DefaultLog
		{
			get
			{
				if (log == null)
				{
					log4net.Config.XmlConfigurator.Configure(); 
					log = LogManager.GetLogger("Exception Reporting");
				}
				return log;
			}
		}
	}
}