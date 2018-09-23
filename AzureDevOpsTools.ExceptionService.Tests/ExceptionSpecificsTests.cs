using AzureDevOpsTools.Exception.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Configuration;
using NUnit.Framework;
using System.Linq;

namespace AzureDevOpsTools.ExceptionService.Common.Tests
{
    public class ExceptionSpecificsTests
    {
        [Test]
        public void InsertConfiguration()
        {
            var c = new ConfigurationStoreCosmosDB();
        }

        [Test]
        public void GetAccountsTest()
        {
            try
            { 
                var c = new TfsStoreWithException();
                var accounts = c.GetAccounts("").Result;
                System.Console.WriteLine(accounts.Count());
            }
            catch( System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }


        [Test]
        public void ThatConstructionWithStringsWorks()
        {
            var sut = new ExceptionSpecifics
            {
                RefCount = "1",
                StackChecksum = "2",
                ExceptionReporter = "3",
                ExceptionType = "4",
                BuildVersion = "5",
                ExceptionMessage = "6a",
                ExceptionMessageEx = "6b",
                Class = "7",
                Method = "8",
                AssemblyName = "9",
                StackTrace = "10",
                Source = "11"
            };
            var res = sut.ToString();
            var sut2 = ExceptionSpecifics.CreateExceptionSpecifics(res);
            Assert.Multiple(() =>
            {
                Assert.That(sut2.RefCount,Is.EqualTo(sut.RefCount));
                Assert.That(sut2.StackChecksum, Is.EqualTo(sut.StackChecksum));
                Assert.That(sut2.ExceptionReporter, Is.EqualTo(sut.ExceptionReporter));
                Assert.That(sut2.ExceptionType, Is.EqualTo(sut.ExceptionType));
                Assert.That(sut2.BuildVersion, Is.EqualTo(sut.BuildVersion));
                Assert.That(sut2.Class, Is.EqualTo(sut.Class));
                Assert.That(sut2.Method, Is.EqualTo(sut.Method));
                Assert.That(sut2.AssemblyName, Is.EqualTo(sut.AssemblyName));
                Assert.That(sut2.StackTrace, Is.EqualTo(sut.StackTrace));
                Assert.That(sut2.Source, Is.EqualTo(sut.Source));
                Assert.That(sut2.ExceptionMessage,Is.EqualTo($"{sut.ExceptionMessage}->->->{sut.ExceptionMessageEx}"));
                Assert.That(sut2.ExceptionMessageEx.Length,Is.EqualTo(0));


            });

        }

        [Test]
        public void ThatIncrementWorks()
        {
            var sut = new ExceptionSpecifics {RefCount = "1"};
            sut.IncrementIncidentCount();
            Assert.That(sut.RefCount,Is.EqualTo("2"));
        }

    }
}
