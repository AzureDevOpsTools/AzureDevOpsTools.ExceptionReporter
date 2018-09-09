using AzureDevOps.Exception.Common;
using AzureDevOps.Exception.Service.Common.Sec;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AzureDevOps.Exception.Service.Common.Stores.TFS
{
    public class WorkItemJson
    {
        private const string Application = "Osiris.Application";
        private const string AssignedToFieldName = "System.AssignedTo";
        private const string CommentFieldName = "System.Description";
        private const string RefCountFieldName = "Osiris.Exception.IncidentCount";
        private const string ExceptionReporterFieldName = "Osiris.Exception.ExceptionReporter";
        private const string BuildVersionFieldName = "Osiris.Exception.BuildVersion";
        private const string ExceptionMessageFieldName = "Osiris.Exception.Message";
        private const string ExceptionMessageExFieldName = "Osiris.Exception.MessageEx";
        private const string ExceptionTypeFieldName = "Osiris.Exception.Type";
        private const string ClassFieldName = "Osiris.Exception.ClassName";
        private const string MethodFieldName = "Osiris.Exception.MethodName";
        private const string SourceFieldName = "Osiris.Exception.Source";
        private const string StackTraceFieldName = "Osiris.Exception.StackTrace";
        private const string StackChecksumFieldName = "Osiris.Exception.StackChecksum";
        private const string AssemblyName = "Inmeta.AssemblyName";


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
            Add(ExceptionMessageExFieldName,exception.ExceptionMessage);

            Add(ExceptionMessageFieldName,TFSStringUtil.GenerateValidTFSStringType(exception.ExceptionMessage));
            Add(ExceptionTypeFieldName, exception.ExceptionType);

            var kmParams = exception.TheClass.Split('|');
            Add(ClassFieldName,kmParams[0]);

            //if (wi.Fields.Contains(AssemblyName) && kmParams.Count() > 1)
            //{
            Add(AssemblyName,kmParams[1]);
            Add(MethodFieldName,exception.TheMethod);
            Add(SourceFieldName,exception.TheSource);
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
