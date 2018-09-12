using log4net;
using log4net.Core;

namespace AzureDevOps.Exception.Common
{

    public class ServiceLog
    {

        public static void Error(string s)
        {
            System.Diagnostics.Trace.TraceError(s);
        }

        public static void Warning(string s)
        {
            System.Diagnostics.Trace.TraceWarning(s);
        }

        public static void Information(string s)
        {
            System.Diagnostics.Trace.TraceInformation(s);
        }

       
    }
}