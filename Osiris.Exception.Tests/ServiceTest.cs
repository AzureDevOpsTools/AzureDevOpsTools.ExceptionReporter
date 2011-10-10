using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inmeta.Exception.Reporter;
using Fasterflect;

namespace Inmeta.Exception.Tests
{
    /// <summary>
    /// Summary description for ServiceTest
    /// </summary>
    [TestClass]
    public class ServiceTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        private const string SettingsFileUri = "Applications.xml";

        [TestInitialize]
        public void MyTestInitialize()
        {
            string xmlContent = ExceptionTestConstants.APPLICATION_CONFIG;
            var finfo = new FileInfo(SettingsFileUri);
            var writer = finfo.CreateText();
            writer.Write(xmlContent);
            writer.Close();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            new FileInfo(SettingsFileUri).Delete();
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        #endregion

        [TestCategory("Integration")]
        [TestMethod]
        public void TFSExceptionReport_TestNewApplicationExceptionCreation()
        {
            try
            {
                throw new ApplicationException("this is my test exception");
            }
            catch (System.Exception ex)
            {
                var exception = new TFSExceptionReport(ExceptionTestConstants.APPLICATION_NAME, "Olav Nybø", "on", ex, "applicationname", "testing");
                exception.CallMethod("Post");                               
            }
        }

    }   
}
