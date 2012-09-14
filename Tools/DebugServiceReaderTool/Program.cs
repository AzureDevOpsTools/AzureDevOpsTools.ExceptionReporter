using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Services;
using Inmeta.Exception.Service.Proxy.Reader;

namespace DebugServiceReaderTool
{
    class Program
    {
        static void Main(string[] args)
        {
            bool done = false;
            while (!done)
            {
                try
                {

                    //using (((System.Security.Principal.WindowsIdentity) HttpContext.Current.User.Identity).Impersonate())
                    {

                        int interval;
                        if (!int.TryParse(ConfigurationManager.AppSettings["pollintervall"], out interval))
                            interval = 60 * 1000;

                        var webSecurityMode = WebHttpSecurityMode.TransportCredentialOnly;
                        Enum.TryParse(ConfigurationManager.AppSettings["httpSecurityMode"], out webSecurityMode);

                        var httpSecurityMode = HttpClientCredentialType.Basic;
                        Enum.TryParse(ConfigurationManager.AppSettings["httpSecurityMode"], out httpSecurityMode);

                        var ServiceSettings = new ProxyReaderServiceSettings(
                            new Uri(ConfigurationManager.AppSettings["serviceURL"]),
                            ConfigurationManager.AppSettings["username"],
                            ConfigurationManager.AppSettings["passord"],
                            ConfigurationManager.AppSettings["domain"],
                            webSecurityMode,
                            httpSecurityMode);



                        var channelFactory = new WebChannelFactory<IGetExceptionsService>(ServiceSettings.ServiceUrl);

                        ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(OnValidate);
                        var wb = channelFactory.Endpoint.Binding as WebHttpBinding;

                        if (wb != null)
                        {
                            wb.Security.Mode = ServiceSettings.HttpSecurityMode;
                            wb.Security.Transport.ClientCredentialType = ServiceSettings.ClientCredentials;
                            wb.MaxReceivedMessageSize = int.MaxValue;
                            //wb.ReaderQuotas.MaxStringContentLength = 65535;
                        }

                        if (channelFactory.Credentials != null)
                        {
                            channelFactory.Credentials.Windows.ClientCredential.Domain   = ServiceSettings.Domain;
                            channelFactory.Credentials.Windows.ClientCredential.Password = ServiceSettings.Password;
                            channelFactory.Credentials.Windows.ClientCredential.UserName = ServiceSettings.Username;

                            channelFactory.Credentials.UserName.UserName = (!String.IsNullOrEmpty(ServiceSettings.Domain) ? ServiceSettings.Domain + "\\" : String.Empty) + ServiceSettings.Username;
                            channelFactory.Credentials.UserName.Password = ServiceSettings.Password;
                        }

                        var channel = channelFactory.CreateChannel();

                        PrintResult(channel);
                    }
                }
                catch (System.Exception exp)
                {
                    Console.WriteLine(exp.ToString());
                }
                done = Console.ReadKey().Key == ConsoleKey.Q;
            }
        }

        private static void PrintResult(IGetExceptionsService channel)
        {
            var exc = channel.GetExceptionsReliable();
            exc.Value.ToList().ForEach((exp) => Console.WriteLine(exp.ToString()));
            channel.AckDelivery(exc.Key);
        }

        public static bool OnValidate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
