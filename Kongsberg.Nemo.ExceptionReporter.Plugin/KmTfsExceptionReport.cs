using System.Linq;
using System.Reflection;
using System;
using System.Diagnostics.Contracts;
using System.Text;
using Inmeta.Exception.Reporter;
using Inmeta.Exception.Reporter.TFSExeptionService;

namespace Inmeta.ExceptionReporter.Km
{
    /// <summary>
    /// This class allows you to post exception reports over the internet.
    /// </summary>
    [Serializable]
    internal class KmTFSExceptionReport
    {
        internal ExceptionEntity ExceptionEntity { get; set; }

        /// <summary>
        /// Create a new exception report item
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="reporter"></param>
        /// <param name="username"></param>
        /// <param name="ex"></param>
        public KmTFSExceptionReport(string applicationName, string reporter, string username, System.Exception ex)
            : this(applicationName, reporter, username, ex, "", "")
        {
            //ensure contracts.
            Contract.Requires(String.IsNullOrEmpty(applicationName));
            Contract.Requires(String.IsNullOrEmpty(reporter));
            Contract.Requires(String.IsNullOrEmpty(username));
            //Pipe to correct ctor.

            //ensure contracts.
            Contract.Ensures(ExceptionEntity != null);
            Contract.Ensures(String.IsNullOrEmpty(ExceptionEntity.ApplicationName));
            Contract.Ensures(String.IsNullOrEmpty(ExceptionEntity.Reporter));
            Contract.Ensures(String.IsNullOrEmpty(ExceptionEntity.Username));
        }

        /// <summary>
        /// Create a new exception repoert item.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="reporter"></param>
        /// <param name="username"></param>
        /// <param name="ex"></param>
        /// <param name="version"></param>
        /// <param name="description">Step to reproduce the error.</param>
        public KmTFSExceptionReport(string applicationName, string reporter, string username, System.Exception ex, string version, string description)
        {
            string stackTrace = CreateExceptionText(ex);
            string title = GetTitle(ex);
            string message = title; 
            string exceptionClass = GetExceptionClass(ex);
            string exceptionMethod = GetExceptionMethod(ex);
            string assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            ExceptionEntity = new ExceptionEntity
            {
                ApplicationName = applicationName,
                Reporter = reporter,
                Username = username,
                Version = version,
                TheSource = ex.Source ?? String.Empty,
                TheClass = exceptionClass + '|' + assemblyName,
                TheMethod = exceptionMethod,
                StackTrace = stackTrace,
                Comment = description,
                ExceptionMessage = message,
                ExceptionType = ex.GetType().ToString(),
                ExceptionTitle = title
            };

        }

        private string GetExceptionMethod(System.Exception ex)
        {
            string exceptionMethod;
            try
            {
                exceptionMethod = (ex.TargetSite ?? MethodBase.GetCurrentMethod()).Name;
            }
            catch (TargetException)
            {
                exceptionMethod = "N/A";
                KmReportLogger.Instance.LogExceptionsDuringDelivery(new TargetException("Class is late bound cannot determine method", ex));
            }
            return exceptionMethod;
        }

        private string GetExceptionClass(System.Exception ex)
        {
            string exceptionClass;
            try
            {
                var declaring = (ex.TargetSite ?? MethodBase.GetCurrentMethod()).DeclaringType;
                exceptionClass = (declaring == null) ? "Global Method" : declaring.FullName;
            }
            catch (TargetException)
            {
                KmReportLogger.Instance.LogExceptionsDuringDelivery(new TargetException("Class is late bound cannot determine class", ex));
                exceptionClass = "N/A";
            }
            return exceptionClass;
        }
        
        private string GetTitle(System.Exception ex)
        {
            System.Exception innerEx = GetMostInnerException(ex);
            var splittedTitle = innerEx.StackTrace.Split(
                new[] { " in " }, StringSplitOptions.RemoveEmptyEntries).First().Trim().Split(
                    new[] { " at " }, StringSplitOptions.RemoveEmptyEntries);

            var title = GetFirstKongsbergLine(splittedTitle).Trim().Split(
                        new[] { "(" }, StringSplitOptions.RemoveEmptyEntries).First().Trim();
            title = innerEx.Message.Trim('.') + (title.Trim().StartsWith("at") ? " " : " at ") + title.Trim();
            title = TFSStringUtil.GenerateValidTFSStringType(title);
            return title;
        }

        private string GetFirstKongsbergLine(string[] splittedTitle)
        {
            foreach (var line in splittedTitle)
            {
                if (line.Trim().ToLower().Contains("kongsberg"))
                    return line;
            }

            return splittedTitle.First();
        }

        /// <summary>
        /// Post the exception report to the service.
        /// </summary>
        /// <returns>If Post fails to deliver the report, the reason is represented in the returning exception. If success return value is null</returns>
        
       
        private System.Exception GetMostInnerException(System.Exception e)
        {
            var temp = e;

            while (temp.InnerException != null)
                temp = temp.InnerException;

            return temp;
        }

        internal string CreateExceptionText(System.Exception e)
        {
            var errorText = new StringBuilder();
            errorText.Append(FormStringFromException(e));

            while (e.InnerException != null)
            {
                e = e.InnerException;
                var textToAppend = FormStringFromException(e);

                if (textToAppend.Length > 0)
                {
                    if (errorText.Length > 0)
                        errorText.Insert(0, textToAppend + Environment.NewLine + "Error: ");
                    else
                        errorText.Append(textToAppend);
                }
            }

            return errorText.ToString();
        }

        private string FormStringFromException(System.Exception ex)
        {
            var name = ex.GetType().Name;

            if (name.Contains("ParseMessageException"))
                return string.Format("{0}\n{1}", ex.Message, ex.StackTrace ?? String.Empty);

            return string.Format("Exception message: {0}\nType: {1}\n{2}", ex.Message, ex.GetType(), ex.StackTrace ?? String.Empty);
        }
    }
}
