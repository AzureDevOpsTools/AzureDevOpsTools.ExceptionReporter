using System.Reflection;
using System;
using System.Diagnostics.Contracts;
using System.Text;
using AzureDevOps.Exception.Common;
using ExceptionEntity = AzureDevOps.Exception.Reporter.TFSExeptionService.ExceptionEntity;

namespace AzureDevOps.Exception.Reporter
{
    /// <summary>
    /// This class allows you to post exception reports over the internet.
    /// </summary>
    [Serializable]
    public class TFSExceptionReport
    {
        public ExceptionEntity ExceptionEntity { get; private set; }

        /// <summary>
        /// Create a new exception report item
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="reporter"></param>
        /// <param name="username"></param>
        /// <param name="ex"></param>
        public TFSExceptionReport(string applicationName, string reporter, string username, System.Exception ex)
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
        public TFSExceptionReport(string applicationName, string reporter, string username, System.Exception ex, string version, string description)
        {
            //ensure contracts.
            Contract.Requires(String.IsNullOrEmpty(applicationName));
            Contract.Requires(String.IsNullOrEmpty(reporter));
            Contract.Requires(String.IsNullOrEmpty(username));
            Contract.Requires(ex != null);

            //ensure contracts.
            Contract.Ensures(ExceptionEntity != null);
            Contract.Ensures(String.IsNullOrEmpty(ExceptionEntity.ApplicationName));
            Contract.Ensures(String.IsNullOrEmpty(ExceptionEntity.Reporter));
            Contract.Ensures(String.IsNullOrEmpty(ExceptionEntity.Username));

            var versionEnsured = version;

            if (String.IsNullOrEmpty(versionEnsured))
                versionEnsured = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var stackTrace = GenerateStackTrace(ex);

            var title = TFSStringUtil.GenerateValidTFSStringType(ex.Message);
            var exceptionClass = String.Empty;

            try
            {
                exceptionClass = (ex.TargetSite ?? MethodBase.GetCurrentMethod()).DeclaringType.FullName;
            }
            catch (TargetException)
            {
                ReportLogger.Instance.LogExceptionsDuringDelivery(new System.Exception("Class is late bound cannot determine class, Class value set to 'N/A'", ex));
                exceptionClass = "N/A";
            }

            var exceptionMethod = String.Empty;

            try
            {
                exceptionMethod = (ex.TargetSite ?? MethodBase.GetCurrentMethod()).Name;
            }
            catch (TargetException)
            {
                exceptionMethod = "N/A";
                ReportLogger.Instance.LogExceptionsDuringDelivery(new System.Exception("Class is late bound cannot determine Method, Method value is set to 'N/A'", ex));
            }

            //create the entity exception.
            ExceptionEntity = new ExceptionEntity()
            {
                ApplicationName = applicationName,
                Reporter = reporter,
                Username = username,
                Version = version,
                TheSource = ex.Source ?? String.Empty,
                TheClass = exceptionClass,
                TheMethod = exceptionMethod,
                StackTrace = stackTrace.ToString(),
                Comment = description,
                ExceptionMessage = ex.Message ?? String.Empty,
                ExceptionType = ex.GetType().ToString(),
                ExceptionTitle = title
            };

        }

        private static StringBuilder GenerateStackTrace(System.Exception ex)
        {
            System.Exception temp = ex.InnerException;

            var stackTrace = new StringBuilder();
            stackTrace.AppendLine(ex.StackTrace);

            while (temp != null)
            {
                stackTrace.AppendLine("Inner StackTrace : ");
                stackTrace.AppendLine(temp.StackTrace);

                temp = temp.InnerException;
            }
            return stackTrace;
        }

        [ContractInvariantMethod]
        private void CodeContract()
        {
            Contract.Invariant(ExceptionEntity != null);
            Contract.Invariant(String.IsNullOrEmpty(ExceptionEntity.ApplicationName));
        }
    
    }
}
