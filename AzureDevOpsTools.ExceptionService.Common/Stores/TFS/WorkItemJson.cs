using AzureDevOpsTools.Exception.Common;
using AzureDevOpsTools.ExceptionService.Common.Sec;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    public class WorkItemJson
    {
        private const string Application = "ExceptionApplication";
        private const string AssignedToFieldName = "System.AssignedTo";
        private const string CommentFieldName = "System.Description";
        private const string RefCountFieldName = "ExceptionIncidentCount";
        private const string ExceptionReporterFieldName = "ExceptionReporter";
        private const string BuildVersionFieldName = "ExceptionBuildVersion";
        private const string ExceptionMessageFieldName = "ExceptionMessage";
        private const string ExceptionTypeFieldName = "ExceptionType";
        private const string ClassFieldName = "ExceptionClass";
        private const string MethodFieldName = "ExceptionMethod";
        private const string SourceFieldName = "ExceptionSource";
        private const string StackTraceFieldName = "ExceptionStackTrace";
        private const string StackChecksumFieldName = "ExceptionStackTraceChecksum";
        private const string AssemblyName = "ExceptionAssemblyName";


        public JsonPatchDocument Json { get; }

        public WorkItemJson(ExceptionEntity exception, IApplicationInfo applicationInfo)
        {
            Json = new JsonPatchDocument();
            Add("System.Title", TFSStringUtil.GenerateValidTFSStringType(exception.ExceptionTitle));
            Add(CommentFieldName, exception.Username + ":\n" + exception.Comment);
            Add("System.AreaPath",applicationInfo.Area);
            Add(Application , exception.ApplicationName);
            Add(AssignedToFieldName , applicationInfo.AssignedTo);
            Add(ExceptionReporterFieldName , exception.Reporter);
            Add(BuildVersionFieldName , exception.Version);
            Add(RefCountFieldName,1.ToString());

            //if (wi.Fields.Contains(ExceptionMessageExFieldName))
            Add(ExceptionMessageFieldName,exception.ExceptionMessage);

            Add(ExceptionMessageFieldName,TFSStringUtil.GenerateValidTFSStringType(exception.ExceptionMessage));
            Add(ExceptionTypeFieldName, exception.ExceptionType);

            var kmParams = exception.ExceptionClass.Split('|');
            Add(ClassFieldName,kmParams[0]);

            //if (wi.Fields.Contains(AssemblyName) && kmParams.Count() > 1)
            //{
            Add(AssemblyName,kmParams[1]);
            Add(MethodFieldName,exception.ExceptionMethod);
            Add(SourceFieldName,exception.ExceptionSource);
            Add(StackTraceFieldName,exception.StackTrace);
            Add(StackChecksumFieldName,Crc32.GetStreamCrc32(exception.StackTrace).ToString());

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
