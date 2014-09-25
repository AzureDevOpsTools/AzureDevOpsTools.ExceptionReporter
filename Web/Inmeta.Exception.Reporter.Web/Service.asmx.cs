using System;
using System.Configuration;
using System.Web;
using System.Web.Services;
using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Services;
using Inmeta.Exception.Service.Common.Stores;
using Inmeta.Exception.Service.Common.Stores.TFS;

namespace Inmeta.Exception.Reporter.Web
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
											theMethod, theSource, changeSet, username));
		}


		[WebMethod]
		public void AddNewApplicationException(ExceptionEntity exceptionEntity)
		{
			SendToStore(exceptionEntity);
		}

        private void SendToStore(ExceptionEntity exceptionEntity)
		{
            bool storeIsTFS = true;
            bool.TryParse(ConfigurationManager.AppSettings["UseTFS"], out storeIsTFS);
            Uri serviceUri = null;
            try
            {
                serviceUri = new Uri(ConfigurationManager.AppSettings["ServiceURL"]);
            }
            catch (System.Exception)
            {
                serviceUri = null;
            }

            var exceptionStore = new ExceptionStore(serviceUri, storeIsTFS);
            exceptionStore.StoreException(exceptionEntity,  new ExceptionSettings(exceptionEntity.ApplicationName,
                                                            HttpContext.Current.Request.MapPath(".") +
                                                             @"\App_Data\Applications.xml"));                
		}
	}
}
