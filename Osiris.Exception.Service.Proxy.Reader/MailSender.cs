using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using Inmeta.Exception.Service.Common.Sec;

namespace Inmeta.Exception.Service.Proxy.Reader
{
    public class MailSender
    {
        private readonly EventLog _log;

        private SmtpClient _client;
        private string _to = string.Empty;

        private static MailSender _sender;
        public static MailSender GetSender(EventLog log)
        {
            return _sender ?? (_sender = new MailSender(log));
        }

        private MailSender(EventLog log)
        {
            _log = log;
            try
            {
                int port;
                if (!int.TryParse(ConfigurationManager.AppSettings["mailServerPort"], out port))
                    port = 25;
                var host = ConfigurationManager.AppSettings["mailServerHost"];
                string user = ConfigurationManager.AppSettings["mailServerLogin"];
                string pass = ConfigurationManager.AppSettings["mailServerPassword"];

                try
                {
                    user = new Encryption().Decrypt(user);
                }
                catch
                {
                    //ignore and use raw value
                }
                
                try
                {
                    pass = new Encryption().Decrypt(pass);
                }
                catch
                {
                    //ignore and use raw value 
                }

                _client = new SmtpClient(host, port);
                _client.Credentials = new System.Net.NetworkCredential(user, pass);
                
                _to = ConfigurationManager.AppSettings["mailServerTo"];

            }
            catch (System.Exception ex)
            {
                EventLogger.LogError(_log, "Mail is configured to be send, but can not be sent : " + ex.Message);
            }
        }

        public void SendMailNotification(string text)
        {
            try
            {
                var message = new MailMessage("NoReply@ExceptionReaderProxy.com", _to);
                message.Subject = "Failed to deliver exception";
                message.Body = text;

                _client.Send(message);
            }
            catch (System.Exception ex)
            {
                EventLogger.LogError(_log, "Mail is configured to be send, but can not be sent : " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            
        }
    }
}
