using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceProcess;
using Inmeta.Exception.Service.Common;
using System.Timers;
using Inmeta.Exception.Service.Common.Services;
using Inmeta.Exception.Service.Common.Stores.TFS;

namespace Inmeta.Exception.Service.Proxy.Reader
{
    public partial class ExceptionReaderService : ServiceBase
    {

        private readonly Timer pollingTimer = new Timer();

        private readonly object pollingMutex = new object(); //Single mutex so only one thing can happen simultaneously: Service Start / Service Stop / Exception Polling (actual work)


        private ProxyReaderServiceSettings _settings;

        private bool _sendMails;

        public ExceptionReaderService()
        {
            InitializeComponent();


            int interval;
            if (!int.TryParse(ConfigurationManager.AppSettings["pollintervall"], out interval))
                interval = 60 * 1000;

            var webSecurityMode = WebHttpSecurityMode.TransportCredentialOnly;
            Enum.TryParse(ConfigurationManager.AppSettings["httpSecurityMode"], out webSecurityMode);

            var httpSecurityMode = HttpClientCredentialType.Basic;
            Enum.TryParse(ConfigurationManager.AppSettings["httpSecurityMode"], out httpSecurityMode);

            _settings = new ProxyReaderServiceSettings(
                new Uri(ConfigurationManager.AppSettings["serviceURL"]),
                ConfigurationManager.AppSettings["username"],
                ConfigurationManager.AppSettings["passord"],
                ConfigurationManager.AppSettings["domain"],
                webSecurityMode,
                httpSecurityMode);

            pollingTimer.Interval = interval;
            pollingTimer.AutoReset = true;
            pollingTimer.Elapsed += PollExceptionQueue;

            ServiceName = "Inmeta Exception Reader Service";
            AutoLog = false;
            
            bool.TryParse(ConfigurationManager.AppSettings["SendMailNotificationOnParseFailure"], out _sendMails);
        }

        protected override void OnStart(string[] args)
        {
            EventLogger.LogInformation(EventLog, "Service starting...");
            lock (pollingMutex)
            {
                pollingTimer.Start();
                EventLogger.LogInformation(EventLog, "Service started.");
            }
        }

        protected override void OnStop()
        {
            EventLogger.LogInformation(EventLog, "Service stopping...");
            lock (pollingMutex)
            {
                pollingTimer.Stop();
                EventLogger.LogInformation(EventLog, "Service stopped.");
            }
        }

        private bool polling;

        private void PollExceptionQueue(object sender, ElapsedEventArgs e)
        {
            if (polling) return;
            lock (pollingMutex)
            {
                if (polling) return;
                try
                {
                    polling = true;
                    if (!pollingTimer.Enabled) return; //Handles the case where the timer fires an event while OnStop is executing. Might not be all that important..

                    SingleThreadedPollExceptionQueue();
                }
                catch (System.Exception ex)
                {
                    EventLogger.LogException(EventLog, ex);
                }
                finally
                {
                    polling = false;
                }
            }
        }

        private void SingleThreadedPollExceptionQueue()
        {

            var failedApplications = new List<string>();

            //read new exceptions from service.
            var response = ReadFromRestService();

            //store them in TFS.
            response.Value.ToList().ForEach((exceptionData) =>
            {
                //if not in failed list and not null.
                if (exceptionData == null || failedApplications.Contains(exceptionData.ApplicationName))
                    return;

                try
                {
                    using (var registrator = new TFSStore())
                        registrator.RegisterException(exceptionData, new ExceptionSettings(exceptionData.ApplicationName, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Applications.xml")));
                }
                catch (System.Exception ex)
                {
                    EventLogger.LogError(EventLog, ex.ToString());

                    //add to failed list to avoid same errors on multiple items.
                    failedApplications.Add(exceptionData.ApplicationName);
                }
            });

            if (_sendMails && response.Value.Any() && !SendAck(response.Key) )
                MailSender.GetSender(EventLog).SendMailNotification("Probably lost exceptions - acknowledgment error. Check file " + response.Key + " at Exception Service Portal");
        }

        private KeyValuePair<string, IEnumerable<ExceptionEntity>> ReadFromRestService()
        {
            try
            {
                var channel = GetChannel();
                return channel.GetExceptionsReliable();
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
        }

        private bool SendAck(string key)
        {
            try
            {
                var channel = GetChannel();
                return channel.AckDelivery(key);
            }
            catch (System.Exception exp)
            {
                EventLogger.LogException(EventLog, exp);
                return false;
            }
        }
        private IGetExceptionsService GetChannel()
        {
            var channelFactory = new WebChannelFactory<IGetExceptionsService>(_settings.ServiceUrl);

            //allways validate certificate.
            ServicePointManager.ServerCertificateValidationCallback = OnValidate;

            var wb = channelFactory.Endpoint.Binding as WebHttpBinding;
            wb.MaxReceivedMessageSize = int.MaxValue;

            //default no HTTPS.
            wb.Security.Mode = _settings.HttpSecurityMode;
            wb.Security.Transport.ClientCredentialType = _settings.ClientCredentials;

            channelFactory.Credentials.Windows.ClientCredential.Password = _settings.Password;
            channelFactory.Credentials.Windows.ClientCredential.UserName = _settings.Username;

            channelFactory.Credentials.UserName.UserName = (!String.IsNullOrEmpty(_settings.Domain)
                                                                ? _settings.Domain + "\\"
                                                                : String.Empty) + _settings.Username;
            channelFactory.Credentials.UserName.Password = _settings.Password;

            var channel = channelFactory.CreateChannel();
            return channel;
        }

        public static bool OnValidate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
