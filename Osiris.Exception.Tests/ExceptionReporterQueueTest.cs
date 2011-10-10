using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Stores.MSMQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inmeta.Exception.Tests
{
    
    
    /// <summary>
    ///This is a test class for ExceptionReporterQueueTest and is intended
    ///to contain all ExceptionReporterQueueTest Unit Tests
    ///</summary>
    [TestClass]
    public class ExceptionReporterQueueTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [Ignore]
        [TestMethod]
        public void ReadQueueLocal()
        {
            using (var queue = new ExceptionQueue()) 
            {
                queue.Purge();
                queue.SendException(new ExceptionEntity());
                var test = queue.CheckForExceptions(10);
                Assert.IsNotNull(test);
            }
        }


        [Ignore]
        [TestMethod]
        public void ReadQueueWs()
        {
            var reader = new ExceptionQueueReader.ExceptionQueueReaderClient();
            var result = reader.GetException();
        }
    }
}
