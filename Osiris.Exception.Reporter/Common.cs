using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace Inmeta.Exception.Reporter
{
    public class Common
    {
        private static Common _instance = null;
        private Common()
        {
        }

        public static Common Instance
        {
            get { return _instance ?? (_instance = new Common()); }
        }

        public virtual string CreateExceptionText(System.Exception e)
        {
            var errorText = new StringBuilder();

            errorText.Append((string) FormStringFromException(e));

            while (e.InnerException != null)
            {
                e = e.InnerException;
                errorText.Insert((int) 0, (string) FormStringFromException(e));
            }

            return errorText.ToString();
        }

        public virtual string FormStringFromException(System.Exception ex)
        {
            return String.Format("{0} at: {1}" + Environment.NewLine, ex.Message, ex.StackTrace ?? "Empty stack trace");
        }

        /// <summary>
        /// Get Reporter
        /// </summary>
        public virtual string Reporter
        {
            get
            {
                try
                {
                    //same as NEMO, but with exception handling.
                    var currentNtUser = WindowsIdentity.GetCurrent();
                    return (currentNtUser != null) ? currentNtUser.Name : "n/a";
                }
                catch (SecurityException privEx)
                {
                    const string temp = "Insufficient privileges to extract the current user.";

                    ReportLogger.Instance.LogExceptionsDuringDelivery(
                        new UnauthorizedAccessException(temp, privEx));

                    return temp;
                }
            }
        }

        /// <summary>
        /// Get VERSION
        /// </summary>
        public virtual string Version
        {
            get
            {
                string version;
                try
                {
                    //SAME AS NEMO, but with exception handling
                    version =
                        Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
                catch (System.Exception privEx)
                {
                    version =
                        "Insufficient privileges to extract the correct assembly version";
                    ReportLogger.Instance.LogExceptionsDuringDelivery(
                        new UnauthorizedAccessException(version, privEx));
                }

                return version;
            }
        }
    }
}
