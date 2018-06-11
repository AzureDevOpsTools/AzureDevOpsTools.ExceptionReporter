using System.IO;
using System.Xml.Linq;
using Inmeta.Exception.Service.Common.Stores.TFS;
using NUnit.Framework;


namespace Inmeta.ExceptionService.TFS.Utils.Tests
{
    
    
    /// <summary>
    ///This is a test class for ExceptionSettingsTest and is intended
    ///to contain all ExceptionSettingsTest Unit Tests
    ///</summary>
    
    public class ExceptionSettingsTest
    {
        private string SettingsFileUri;


        private const string settingsFileUri = "Applications.xml";
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

        [SetUp]
        public void MyTestInitialize()
        {
            SettingsFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, settingsFileUri);
            var finfo = new FileInfo(SettingsFileUri);
            var writer = finfo.CreateText();
            writer.Write(XmlContent);
            writer.Close();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            new FileInfo(SettingsFileUri).Delete();
        }


        /// <summary>
        ///A test for ExceptionSettings Constructor
        ///</summary>
        [Test]
        public void ExceptionSettingsConstructorTest()
        {
            var settings = new ExceptionSettings(string.Empty, SettingsFileUri);
            Assert.IsNotNull(settings);
        }

        [Test]
        public void AssignedTo_Is_Default_User_When_Exception_Has_No_Id()
        {
            var target = new ExceptionSettings(string.Empty, SettingsFileUri);

            Assert.AreEqual("Default user", target.AssignedTo);
        }

        [Test]
        public void AssignedTo_Is_Jakob_Ehn_When_Application_Is_MyApp()
        {
            var target = new ExceptionSettings("My.App", SettingsFileUri);

            Assert.AreEqual("Jakob Ehn", target.AssignedTo);
        }
    }
}
