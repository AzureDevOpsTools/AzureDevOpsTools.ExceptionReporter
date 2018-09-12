using System.IO;
using AzureDevOps.Exception.Service.Common.Stores.TFS;
using NUnit.Framework;


namespace AzureDevOps.ExceptionService.TFS.Utils.Tests
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
            var settings = new ExceptionSettings(string.Empty);
            Assert.IsNotNull(settings);
        }

    }
}
