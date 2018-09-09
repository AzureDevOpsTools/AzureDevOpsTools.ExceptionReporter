using System;
using AzureDevOps.Exception.Service.Common.Sec;

namespace AzureDevOps.Exception.Service.Common
{
    /// <summary>
    /// This class contains setting for connecting to the exception service.
    /// This is set in the appSettings section. 
    ///     legal values:
    ///        key = serviceURL, value = complete url to exception reporter service (http://[SERVER]/[PATH]/service.asmx)
    /// </summary>
    public class ServiceSettings
    {
        public ServiceSettings(Uri serviceUrl, string username, string password, string domain = "")
        {
            ServiceUrl = serviceUrl;
            Username = username;
            try
            {
                Username = new Encryption().Decrypt(Username);
            }
            catch
            {
                //ignore and use raw value
            }

            Password = password;
            try
            {
                Password = new Encryption().Decrypt(Password);
            }
            catch
            {
                //ignore and use raw value 
            }

            Domain = domain;
        }

        /// <summary>
        /// If set from code it will have presedence over any setting in config file <see cref="ServiceSettings"/>
        /// </summary>
        public Uri ServiceUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// use UsernameAndPasswordEncryption project to generate encrypted values to insert into [applicationname].exe.config.
        /// </summary>
        public string Username
        {
            private set;
            get;
        }

        /// <summary>
        /// use UsernameAndPasswordEncryption project to generate encrypted values to insert into [applicationname].exe.config.
        /// </summary>
        public string Password
        {
            get;
            private set;
        }

        public string Domain { get; private set; }
        /*
     * DEBUG:
         
     public static readonly string ServiceUrl = "http://localhost:13501/Service.asmx";

            internal static readonly string Username = "";
            internal static readonly string Password = "";
            internal static readonly string Domain = ""; 
 
     PRODUCTION 
                
     public static readonly string ServiceUrl = "http://exceptions.osiris.no/Service.asmx";
        
            internal static readonly string Username = "OsirisErrorReporter";
            internal static readonly string Password = "1qaz2WSX";
            internal static readonly string Domain = "";    

 
     */
    }
}
