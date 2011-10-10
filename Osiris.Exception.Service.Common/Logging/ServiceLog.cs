using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace Inmeta.Exception.Common
{

	public class ServiceLog
	{
		private static ILog _log;

		public static ILog DefaultLog
		{
			get
			{
				if (_log == null)
				{
					log4net.Config.XmlConfigurator.Configure(); 
					_log = log4net.LogManager.GetLogger("Exception Reporting");
				}
				return _log;
			}
		}
	}
}