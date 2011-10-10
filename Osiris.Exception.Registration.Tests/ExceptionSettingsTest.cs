using System;
using System.IO;
using Osiris.Exception.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osiris.Exception.Service.Common;

namespace Osiris.Exception.Registration.Tests
{


    /// <summary>
    ///This is a test class for ExceptionSettingsTest and is intended
    ///to contain all ExceptionSettingsTest Unit Tests
    ///</summary>
    [TestClass]
    public class ExceptionSettingsTest
    {
        private const string SettingsFileUri = "Applications.xml";
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //[ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {

        }

        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void MyTestInitialize()
        {
            const string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <Applications>
                        <Application Name=""Default"">
                            <TFSServer>http://vm-tfs2010-test:8080/tfs</TFSServer>
                            <Collection>Default</Collection>
                            <TeamProject>XDemo</TeamProject>
                            <Area>Some Area</Area>
                            <AssignedTo>Default user</AssignedTo>
                            <Username>oslabadmin</Username>
                            <Domain>os-lab</Domain>
                            <Password>Y67uJi)9</Password>
                        </Application>
                        <Application Name=""My.App"">
                            <TFSServer>http://vm-tfs2010-test:8080/tfs</TFSServer>
                            <Collection>Deafult</Collection>
                            <TeamProject>XDemo</TeamProject>
                            <Area>Some Area</Area>
                            <AssignedTo>Jakob Ehn</AssignedTo>
                            <Username>oslabadmin</Username>
                            <Domain>os-lab</Domain>
                            <Password>Y67uJi)9</Password>
                        </Application>
                    </Applications>";
            var finfo = new FileInfo(SettingsFileUri);
            var writer = finfo.CreateText();
            writer.Write(xmlContent);
            writer.Close();            
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup]
        public void MyTestCleanup()
        {
            new FileInfo(SettingsFileUri).Delete();            
        }
        //
        #endregion


        [TestMethod]
        public void AssignedTo_Is_Default_User_When_Exception_Has_No_Id()
        {
            var exception = new ExceptionEntity();

            var target = new ExceptionSettings(exception.ApplicationName, SettingsFileUri);

            Assert.AreEqual("Default user", target.AssignedTo);
        }

        [TestMethod]
        public void AssignedTo_Is_Jakob_Ehn_When_Application_Is_MyApp()
        {
            var exception = new ExceptionEntity {ApplicationName = "My.App"};

            var target = new ExceptionSettings(exception.ApplicationName, SettingsFileUri);

            Assert.AreEqual("Jakob Ehn", target.AssignedTo);
        }
    }
}
