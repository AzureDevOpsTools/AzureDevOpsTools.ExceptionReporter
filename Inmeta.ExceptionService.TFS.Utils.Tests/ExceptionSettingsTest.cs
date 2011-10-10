using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Inmeta.ExceptionService.TFS.Utils.Tests
{
    
    
    /// <summary>
    ///This is a test class for ExceptionSettingsTest and is intended
    ///to contain all ExceptionSettingsTest Unit Tests
    ///</summary>
    [TestClass]
    public class ExceptionSettingsTest
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

        private const string SettingsFileUri = "Applications.xml";
        private const string XmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <Applications>
                        <Application Name=""Default"">
                            <TFSServer>http://tfs.osiris.no:8080/tfs</TFSServer>
                            <Collection>LabCollection</Collection>
                            <TeamProject>Oistein_KM_Sim_Upgrade</TeamProject>
                            <Area>Oistein_KM_Sim_Upgrade\Modul 2\</Area>
                            <AssignedTo>Default user</AssignedTo>
                        </Application>
                        <Application Name=""Osiris.My.Test.App"">
                            <TFSServer>http://tfs.osiris.no:8080/tfs</TFSServer>
                            <Collection>LabCollection</Collection>
                            <TeamProject>Oistein_KM_Sim_Upgrade</TeamProject>
                            <Area>Oistein_KM_Sim_Upgrade\Modul 1\</Area>
                            <AssignedTo>Olav Nybø</AssignedTo>
                        </Application>
                        <Application Name=""My.App"">
                            <TFSServer>http://tfs.osiris.no:8080/tfs</TFSServer>
                            <Collection>LabCollection</Collection>
                            <TeamProject>Oistein_KM_Sim_Upgrade</TeamProject>
                            <Area>Oistein_KM_Sim_Upgrade\Modul 1\</Area>
                            <AssignedTo>Jakob Ehn</AssignedTo>
                        </Application>
                    </Applications>";

        [TestInitialize]
        public void MyTestInitialize()
        {            
            var finfo = new FileInfo(SettingsFileUri);
            var writer = finfo.CreateText();
            writer.Write(XmlContent);
            writer.Close();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            new FileInfo(SettingsFileUri).Delete();
        }


        /// <summary>
        ///A test for ExceptionSettings Constructor
        ///</summary>
        [TestMethod]
        public void ExceptionSettingsConstructorTest()
        {
            var settings = new ExceptionSettings(string.Empty, SettingsFileUri);
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void AssignedTo_Is_Default_User_When_Exception_Has_No_Id()
        {
            var target = new ExceptionSettings(string.Empty, SettingsFileUri);

            Assert.AreEqual("Default user", target.AssignedTo);
        }

        [TestMethod]
        public void AssignedTo_Is_Jakob_Ehn_When_Application_Is_MyApp()
        {
            var target = new ExceptionSettings("My.App", SettingsFileUri);

            Assert.AreEqual("Jakob Ehn", target.AssignedTo);
        }
    }
}
