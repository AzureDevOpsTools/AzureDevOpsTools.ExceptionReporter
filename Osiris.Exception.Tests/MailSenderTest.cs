using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inmeta.Exception.Service.Proxy.Reader;

namespace Inmeta.Exception.Tests
{
    [TestClass]
    public class MailSenderTest
    {
        [TestMethod]
        public void TestSendMail()
        {
            var sender = MailSender.GetSender(new EventLog("testLog"));
            sender.SendMailNotification("test");
        }
    }
}
