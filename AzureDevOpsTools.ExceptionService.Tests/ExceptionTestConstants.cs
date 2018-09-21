using System;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AzureDevOpsTools.Exception.Tests
{
    internal static class ExceptionTestConstants
    {
        public static readonly string PROJECT = "Lars_KM_sim_exception_test";
        public static readonly string TFSSERVER = "http://vm-osiris-tfs2010:8080/tfs";
        public static readonly string APPLICATION_CONFIG = String.Empty;
        public static readonly string APPLICATION_NAME = "Inmeta.TESTAPP";
        public static readonly string AREA = PROJECT;


        static ExceptionTestConstants()
        {
            StringBuilder applicationString = new StringBuilder();
            applicationString.AppendFormat(@"<?xml version=""1.0"" encoding=""utf-8"" ?>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                    <Applications>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                        <Application Name=""Default"">{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                            <TFSServer>");
            applicationString.AppendFormat(TFSSERVER);
            applicationString.AppendFormat(@"</TFSServer>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <Collection>DefaultCollection</Collection>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                            <TeamProject>");
            applicationString.AppendFormat(PROJECT);
            applicationString.AppendFormat(@"</TeamProject>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                            <Area>");
            applicationString.AppendFormat(AREA);
            applicationString.AppendFormat(@"</Area>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                            <AssignedTo>oslabadmin</AssignedTo>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <Username>oslabadmin</Username>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <Domain>os-lab</Domain>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <Password>Y67uJi)9</Password>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                        </Application>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                        <Application Name=""");
            applicationString.AppendFormat(APPLICATION_NAME);
            applicationString.AppendFormat(@""">{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <TFSServer>");
            applicationString.AppendFormat(TFSSERVER);
            applicationString.AppendFormat(@"</TFSServer>{0}", Environment.NewLine); 
            applicationString.AppendFormat(@"                            <Collection>DefaultCollection</Collection>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                            <TeamProject>");
            applicationString.AppendFormat(PROJECT);
            applicationString.AppendFormat(@"</TeamProject>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <Area>");
            applicationString.AppendFormat(AREA);
            applicationString.AppendFormat(@"</Area>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                            <AssignedTo>oslabadmin</AssignedTo>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                            <Username>oslabadmin</Username>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <Domain>os-lab</Domain>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                            <Password>Y67uJi)9</Password>{0}",Environment.NewLine);
            applicationString.AppendFormat(@"                        </Application>{0}", Environment.NewLine);
            applicationString.AppendFormat(@"                    </Applications>");

            APPLICATION_CONFIG = applicationString.ToString();
        }


        //generates a random (garbage) string.
        public static string RndStr
        {
            get { return RndStrLength(3); }
        }

        public static string RndStrLength(int len)
        {
            Contract.Requires(len > 0);
            Contract.Ensures(Contract.Result<string>() != null);
            Contract.Ensures(Contract.Result<string>().Length == len);
            
            if (random == null)
                random = new Random();

            var temp = new string(Enumerable.Range(0, len).Select(i => (char)('A' + random.Next(30))).ToArray());

            if (temp.Length != len)
                throw new ArgumentOutOfRangeException();
            return temp;
        }


        public static System.Exception GenerateStackTrace(int version, int depth)
        {
            System.Exception res;
            if (depth == 0)
            {
                try
                {
                    throw new System.Exception(version.ToString());
                }
                catch (System.Exception ex)
                {
                    res = new System.Exception("Inner Exception", ex);
                }
            }
            else
            {
                try
                {
                    throw new System.Exception(version.ToString(),
                                               GenerateStackTrace(version + 1, depth - 1));
                }
                catch (System.Exception ex)
                {
                    res = new System.Exception("Inner Exception", ex);
                }
            }

            return res;
        }


        private static Random random { get; set; }

    }

}
