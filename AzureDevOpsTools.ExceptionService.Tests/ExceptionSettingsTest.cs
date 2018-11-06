using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureDevOpsTools.Exception.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using NUnit.Framework;


namespace AzureDevOpsTools.ExceptionService.TFS.Utils.Tests
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

        [Test]
        public async Task CreateWorkItemType()
        {
            var pt = new ProcessTemplate("https://whateveryousay.visualstudio.com", "gvdiglh3bcvi3silg5hgkqhqia5b6635ff6ek64vcubullzfldvq");
            var process = await pt.GetProcessByName("AgileWithExceptions");
            await pt.EnsureExceptionWorkItemType(process.TypeId, "AgileWithExceptions", "Exception6");
        }

        [Test]
        public async Task GetWorkItemType()
        {
            var pt = new ProcessTemplate("https://whateveryousay.visualstudio.com", "gvdiglh3bcvi3silg5hgkqhqia5b6635ff6ek64vcubullzfldvq");
            var process = await pt.GetProcessByName("AgileWithExceptions");
            ProcessWorkItemType wit = await pt.GetWorkItemType(process.TypeId, "AgileWithExceptions.Bug");
            ProcessWorkItemType witBug = await pt.GetWorkItemType(process.TypeId, "AgileWithExceptions.Bug");

//            FormLayout layout = witProcessClient.GetFormLayoutAsync(processId, workItemTypeReferenceName).Result;
        }

        [Test]
        public async Task DeleteWorkItemType()
        {
            var pt = new ProcessTemplate("https://whateveryousay.visualstudio.com", "gvdiglh3bcvi3silg5hgkqhqia5b6635ff6ek64vcubullzfldvq");
            var process = await pt.GetProcessByName("AgileWithExceptions");
            await pt.DeleteExceptionWorkItemType(process.TypeId, "AgileWithExceptions.Exception6");
        }

        [TearDown]
        public void MyTestCleanup()
        {
            new FileInfo(SettingsFileUri).Delete();
        }



    }
}
