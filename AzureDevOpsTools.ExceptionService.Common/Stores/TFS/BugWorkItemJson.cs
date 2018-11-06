using AzureDevOpsTools.Exception.Common;
using AzureDevOpsTools.ExceptionService.Common.Sec;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Text;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    public class BugWorkItemJson
    {

        //Workitem-types:
        private const string ExceptionWorkItemType = "Bug";

        //Workitem-fields:
        //   private const string Application = "Osiris.Application";
        private const string AssignedToFieldName = "System.AssignedTo";
        private const string Description = "System.Description";

        private const string ReproSteps = "Microsoft.VSTS.TCM.ReproSteps";

        private const string SystemInfo = "Microsoft.VSTS.TCM.SystemInfo";

        private const string AcceptanceCriteria = "Microsoft.VSTS.Common.AcceptanceCriteria";

        private const string FoundInBuild = "";
        //   private const string RefCountFieldName = "Osiris.Exception.IncidentCount";
        //   private const string ExceptionReporterFieldName = "Osiris.Exception.ExceptionReporter";
        //private const string BuildVersionFieldName = "Osiris.Exception.BuildVersion";
        //private const string ExceptionMessageFieldName = "Osiris.Exception.Message";
        //private const string ExceptionMessageExFieldName = "Osiris.Exception.MessageEx";
        //private const string ExceptionTypeFieldName = "Osiris.Exception.Type";
        //private const string ClassFieldName = "Osiris.Exception.ClassName";
        //private const string MethodFieldName = "Osiris.Exception.MethodName";
        //private const string SourceFieldName = "Osiris.Exception.Source";
        //private const string StackTraceFieldName = "Osiris.Exception.StackTrace";
        //private const string StackChecksumFieldName = "Osiris.Exception.StackChecksum";
        //private const string AssemblyName = "Inmeta.AssemblyName";


        public JsonPatchDocument Json { get; }

        public BugWorkItemJson(ExceptionEntity exception, IApplicationInfo applicationInfo)
        {
            Json = new JsonPatchDocument();
            Add("System.Title", TFSStringUtil.GenerateValidTFSStringType(exception.ExceptionTitle));
            Add("System.AreaPath", applicationInfo.Area);

            var reproSteps = new StringBuilder();
            reproSteps.AppendLine("");


            //Add(CommentFieldName, exception.Username + ":\n" + exception.Comment);
            //Add(Application , exception.ApplicationName);
            //Add(AssignedToFieldName , applicationInfo.AssignedTo);
            //Add(ExceptionReporterFieldName , exception.Reporter);
            //Add(BuildVersionFieldName , exception.Version);
            //Add(RefCountFieldName,1.ToString());

            //if (wi.Fields.Contains(ExceptionMessageExFieldName))
            //Add(ExceptionMessageFieldName,exception.ExceptionMessage);

            //Add(ExceptionMessageFieldName,TFSStringUtil.GenerateValidTFSStringType(exception.ExceptionMessage));
            //Add(ExceptionTypeFieldName, exception.ExceptionType);

            //var kmParams = exception.ExceptionClass.Split('|');
            //Add(ClassFieldName,kmParams[0]);

            ////if (wi.Fields.Contains(AssemblyName) && kmParams.Count() > 1)
            ////{
            //Add(AssemblyName,kmParams[1]);
            //Add(MethodFieldName,exception.ExceptionMethod);
            //Add(SourceFieldName,exception.ExceptionSource);
            //Add(StackTraceFieldName,exception.StackTrace);
            //Add(StackChecksumFieldName,Crc32.GetStreamCrc32(exception.StackTrace).ToString());

        }

        private void Add(string field, string value)
        {
            Json.Add(new JsonPatchOperation
            {
                Operation=Operation.Add,
                Path = $"/fields/{field}",
                Value=value
            });
        }
    }
}
