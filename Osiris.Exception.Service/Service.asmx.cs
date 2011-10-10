using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Inmeta.Exception.Common;
using Inmeta.Exception.Service.Common;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Inmeta.Exception.Service.Common.FileStore;
using Inmeta.Exception.Service.Common.TFS;

namespace Inmeta.Exception.Service
{
	/// <summary>
	/// Summary description for Service
	/// TODO: Handle team project names on this format: "SomeRandomString; MyTeamProject"
	///         The text before the semicolon should be stripped away, it is used for TFS server routing (See Exception Service Proxy Reader).
	/// 
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class Service : WebService, IExceptionService
	{
        private object _fileLockObject = new object();
		private bool _storeIsTFS;

		public Service()
		{
			//Uncomment the following line if using designed components 
			//InitializeComponent();

			//default true
			_storeIsTFS = true;
	 
			//'UseTFS' = true : 
			//      1. UseTFS do not exists.
			//      2. value not parsable
			//      3. Value is true
			//'UseTFS' = false :
			//      1. UseTFS exists 
			//      2. UseTFS value is 'false' (case insensitive)    

			bool.TryParse(ConfigurationManager.AppSettings["UseTFS"], out _storeIsTFS);
		}

		/// <summary>
		/// Creates an Exception-workitem in TFS with the specified information.
		/// </summary>
		/// <param name="teamProject">The team project which the exception-workitem should be created in.</param>
		/// <param name="reporter">Normally the windows username of the user which reported the application.</param>
		/// <param name="comment">Comment from the user, supposed to contain extra information for reproducing the exception.</param>
		/// <param name="version">Version of the program which had an unhandled exception.</param>
		/// <param name="exceptionMessage">The Message-property of the exception to report.</param>
		/// <param name="exceptionType">The type of the exception to report.</param>
		/// <param name="exceptionTitle">The title of the exception-workitem which will be created.</param>
		/// <param name="stackTrace">Stack-trace of the exception to report.</param>
		/// <param name="theClass">The class where the exception occurred.</param>
		/// <param name="theMethod">The method where the exception occurred.</param>
		/// <param name="theSource">Value of the Exception.Source property. Not sure what this contains.</param>
		/// <param name="changeSet">Not in use</param>
		/// <param name="username">Name of the application that crashed, and an eventual username (in the program) for the user who reported the exception.</param>
		[WebMethod]
		public void AddNewException(string teamProject, string reporter, string comment, string version, string exceptionMessage,
			string exceptionType, string exceptionTitle, string stackTrace, string theClass, string theMethod, string theSource,
			string changeSet, string username)
		{

			SendToStore(new ExceptionEntity(teamProject, reporter, comment, version,
											exceptionMessage,
											exceptionType, exceptionTitle, stackTrace,
											theClass,
											theMethod, theSource, string.Empty, username));
		}


		[WebMethod]
		public void AddNewApplicationException(ExceptionEntity exceptionEntity)
		{
			SendToStore(exceptionEntity);
		}

		[WebMethod]
		public ExceptionEntity[] GetExceptions()
		{
            return ReadFromFile();
                
                //ReadFromMSMQ();
		}

	    private ExceptionEntity[] ReadFromFile()
	    {
            lock (_fileLockObject)
            {
                return new ExceptionEntity[0];
            }
	    }

/*	    private ExceptionEntity ReadFromMSMQ()
	    {
	        try
	        {
	            using (var queue = new ExceptionQueue())
	            {
	                return queue.PopException(SecondsToWait());
	            }
	        }
	        catch (System.Exception ex)
	        {
	            ServiceLog.DefaultLog.Error("Error getting exception from MSMQ : ", ex);
	            throw;
	        }
	    }
        */
	    /*
        private static int SecondsToWait()
		{
			return int.Parse(ConfigurationManager.AppSettings["ExceptionReadTimeout"] ?? "5");
		}*/ 

		private void SendToStore(ExceptionEntity exceptionEntity)
		{
			try
			{
                if (_storeIsTFS)
                    StoreInTFS(exceptionEntity);
                
                StoreInFile(exceptionEntity);
			}
			catch (System.Exception ex)
			{
				ServiceLog.DefaultLog.Error("Error adding new Exception: ", ex);
				throw;
			}
		}

        private void StoreInFile(ExceptionEntity exceptionEntity)
        {
            new FileStore().SaveException(exceptionEntity);
        }

/*	utgår gitt nye krav.	
 * private void StoreInMSMQ(ExceptionEntity exceptionEntity)
		{
			using (var exceptionQueue = new ExceptionQueue())
			{
				exceptionQueue.SendException(exceptionEntity);
			}
		}
        */

		private void StoreInTFS(ExceptionEntity exceptionEntity)
		{
			using (var registrator = new TFSExceptionRegistrator())
			{
				var settings = new ExceptionSettings(exceptionEntity.ApplicationName,
													 HttpContext.Current.Request.MapPath(".") +
													 @"\App_Data\Applications.xml");
				registrator.RegisterException(exceptionEntity, settings);
			}
		}
	}
}
