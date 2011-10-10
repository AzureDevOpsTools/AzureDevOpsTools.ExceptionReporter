using System;
using System.Web.Services;
using Osiris.Exception.Service.Common;

namespace Osiris.Exception.Service.Proxy
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service : WebService, IExceptionService
    {
        [Obsolete("Use AddNewApplicationException")]
        [WebMethod]
        public void AddNewException(string teamProject, string reporter, string comment, string version, 
            string exceptionMessage, string exceptionType, string exceptionTitle, string stackTrace, string theClass, 
            string theMethod, string theSource, string changeSet, string username)
        {
            var exception = new ExceptionEntity(teamProject, reporter, comment, version, exceptionMessage, exceptionType, exceptionTitle, stackTrace, theClass, theMethod, theSource, changeSet, username);

            var exceptionQueue = ExceptionQueue.OpenLocal();

            exceptionQueue.SendException(exception);
        }

        [WebMethod]
        public void AddNewApplicationException(ExceptionEntity exception)
        {
            var exceptionQueue = ExceptionQueue.OpenLocal();

            exceptionQueue.SendException(exception);
        }
    }
}
