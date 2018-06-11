using System.Diagnostics;
using Inmeta.Exception.Service.Proxy.Reader;
using NUnit.Framework;

namespace Inmeta.Exception.Tests
{
    
    public class MailSenderTest
    {
        [Test]
        [Ignore("for now")]
        public void TestSendMail()
        {
            var sender = MailSender.GetSender(new EventLog("testLog"));
            sender.SendMailNotification("test");
        }
    }
}
