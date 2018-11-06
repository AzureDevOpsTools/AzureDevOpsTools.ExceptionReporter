using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    public class ProcessTemplate
    {
        private WorkItemTrackingProcessHttpClient witProcessClient;
        private WorkItemTrackingHttpClient witClient;

        public ProcessTemplate(string accountUri, string token)
        {
            var Credentials = new VssBasicCredential("", token);
            VssConnection connection = new VssConnection(new System.Uri("https://whateveryousay.visualstudio.com/"), Credentials);
            witProcessClient = connection.GetClient<WorkItemTrackingProcessHttpClient>();
            witClient = connection.GetClient<WorkItemTrackingHttpClient>();

        }
        public async Task EnsureExceptionWorkItemType(Guid processId, string processName, string workItemTypeName)
        {
            var witReferenceName = $"{processName}.{workItemTypeName}";
            var processWorkItemType = await GetWorkItemType(processId, witReferenceName);
            if( processWorkItemType == null)
            { 
                var createWorkItemType = new CreateProcessWorkItemTypeRequest()
                {
                    Name = workItemTypeName,
                    Description = "Used for tracking unhandled application exceptions",
                    Color = "f6546a",
                    Icon = "icon_airplane",
                    //InheritsFrom = "Microsoft.VSTS.WorkItemTypes.Bug"
                };
                processWorkItemType = await witProcessClient.CreateProcessWorkItemTypeAsync(createWorkItemType, processId);
            }
            
            await EnsureFieldExists("Custom.ExceptionApplication", "Application", "Application", processWorkItemType.ReferenceName, processId);


            //Add page
            var page = witProcessClient.AddPageAsync(new Page()
            {
                Label = "Exception",
                PageType = PageType.Custom,
                
                Sections = new List<Section>()
                {
                    new Section()
                    {          
                        Id = "Section1",
                        Groups = new List<Group>()
                        {
                            new Group()
                            {
                             Label = "MyGroup",
                             Controls = new List<Control>()
                             {
                                 new Control()
                                 {
                                     Id = "Custom.ExceptionApplication",
                                     Label = "Application",
                                     ControlType = "FieldControl",
                                     Name = "Application",
                                     Visible = true
                                 }
                             }
                            }
                        }
                    }
                }
            }, processId, processWorkItemType.ReferenceName).Result;

            ////Add fields
            //var fields = new string[]
            //{
            //            TfsStoreWithException.Application,
            //            TfsStoreWithException.AssignedToFieldName,
            //            TfsStoreWithException.CommentFieldName,
            //            TfsStoreWithException.RefCountFieldName,
            //            TfsStoreWithException.ExceptionReporterFieldName,
            //            TfsStoreWithException.BuildVersionFieldName,
            //            TfsStoreWithException.ExceptionMessageFieldName,
            //            TfsStoreWithException.ExceptionTypeFieldName,
            //            TfsStoreWithException.ClassFieldName,
            //            TfsStoreWithException.MethodFieldName,
            //            TfsStoreWithException.SourceFieldName,
            //            TfsStoreWithException.StackTraceFieldName,
            //            TfsStoreWithException.StackChecksumFieldName,
            //            TfsStoreWithException.AssemblyName
            //    };

        }

        public async Task DeleteExceptionWorkItemType(Guid processId, string workItemTypeReferenceName)
        {
            await witProcessClient.DeleteProcessWorkItemTypeAsync(processId, workItemTypeReferenceName);
        }

        public async Task<ProcessWorkItemType> GetWorkItemType(Guid processId, string workItemTypeReferenceName)
        {
            var types = await witProcessClient.GetProcessWorkItemTypesAsync(processId);
            var type = types.FirstOrDefault(t => t.ReferenceName == workItemTypeReferenceName);

            //FormLayout layout = witProcessClient.GetFormLayoutAsync(processId, workItemTypeReferenceName).Result;

            //IList<Page> pages = layout.Pages;

            //foreach (Page page in pages)
            //{
            //    Console.WriteLine("{0} ({1})", page.Label, page.Id);

            //    foreach (Section section in page.Sections)
            //    {
            //        Console.WriteLine("    {0}", section.Id);

            //        foreach (Group group in section.Groups)
            //        {
            //            Console.WriteLine("        {0} ({1})", group.Label, group.Id);
            //        }
            //    }
            //}

            return type;

        }
        public async Task EnsureFieldExists(string fieldRefName, string name, string description, string workItemType, Guid processId)
        {
            var field = await witClient.GetFieldAsync(fieldRefName);
            if (field == null)
            {
                field = await witClient.CreateFieldAsync(new WorkItemField()
                {
                    Name = name,
                    Description = description,
                    ReferenceName = fieldRefName
                });
            }
            await witProcessClient.AddFieldToWorkItemTypeAsync(
                new AddProcessWorkItemTypeFieldRequest()
                {
                    ReferenceName = field.ReferenceName
                }, processId, workItemType);


        }

        public async Task<ProcessInfo> GetProcessByName(string processName)
        {
            var processes = await witProcessClient.GetListOfProcessesAsync();
            return processes.FirstOrDefault(p => p.Name.ToLower() == processName.ToLower());
        }
    }
}
