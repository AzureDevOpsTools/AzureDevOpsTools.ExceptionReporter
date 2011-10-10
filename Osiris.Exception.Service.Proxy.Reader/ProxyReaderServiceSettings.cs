
using System;
using System.Configuration;
using System.ServiceModel;
using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Sec;

namespace Inmeta.Exception.Service.Proxy.Reader
{

    /// <summary>
    /// 
    /// This class contains setting for connecting to the exception service.
    /// This file is replaced with the appropriate file during build, when building for test/production.
    /// 
    /// This sort of thing is usually handled with separate app.config files, but it is preferred
    /// for this project to produce an independent DLL, with the correct settings compiled in.
    /// 
    /// </summary>
    public class ProxyReaderServiceSettings : ServiceSettings
    {
        public ProxyReaderServiceSettings(Uri serviceUrl, string username, string password, string domain
            , WebHttpSecurityMode webHttpSecurityMode
            , HttpClientCredentialType httpClientCredentialType) 
            : base(serviceUrl, username, password, domain)
        {
            HttpSecurityMode = webHttpSecurityMode;
            ClientCredentials = httpClientCredentialType;
        }

        public WebHttpSecurityMode HttpSecurityMode
        {
            get;
            private set;
        }

        public HttpClientCredentialType ClientCredentials
        {
            get;
            private set;
        }
    }
}

