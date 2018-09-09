using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Text;
using AzureDevOps.Exception.Service.Common.Sec;
using AzureDevOps.Exception.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOps.Exception.Service.Common.Stores.TFS
{
    public class TfsStoreWithException : AccessToVsts
    {
        //Workitem-types:
        private const string ExceptionWorkItemType = "Exception";

        //Workitem-fields:
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

        public TfsStoreWithException() : base()
        {
            
        }


        public void RegisterException(ExceptionEntity exceptionEntity, IApplicationInfo applicationInfo)
        {
            WorkItem wi = null;
            WorkItem linkedWi = null;
            var workItems = new ExceptionWorkItemCollection(exceptionEntity);

            if (!workItems.HasOpenWorkItems)
            {
                if (workItems.HasWorkItemsWithHigherChangeset)
                {
                    wi = workItems.GetWorkItemWithHigherChangeset();
                    // If not Open and new exception has higher version, we should create new wi with link to the old one
                    if (HasHigherVersion(exceptionEntity.Version, wi))
                    {
                        linkedWi = wi;
                        wi = null;
                    }
                }
                else
                {
                    wi = workItems.GetLatestNotOpenWorkItem();
                }
            }
            else
            {
                wi = workItems.OpenWorkItems.FirstOrDefault();
                if (wi != null && HasLowerVersion(exceptionEntity.Version, wi))
                {
                    ServiceLog.Information(
                        $"Exception with newer version already exists with ID={wi.Id}. Coming exception will be dropped.");
                    return;
                }
            }

            if (wi == null)
            {
                var json = CreateNewException(exceptionEntity, applicationInfo);
                SendException(json);
                return;
            }
            else
            {
                wi = UpdateExisitingWorkItem(wi, exceptionEntity);
            }

            if (wi == null)
                return;

            var errors = wi.Validate();
            if (!errors.Any())
            {
                ServiceLog.Error($"Could not create a workitem but no errors found in it  {wi.Id}");
            }
            else
            {
                var errorString = new StringBuilder();
                foreach (var f in errors)
                    errorString.AppendLine($"Error: {f}");
                ServiceLog.Warning("Workitem failed validation: " + errorString);

                //LARS: should throw exception when exception is not valid:
                throw new ArgumentException("TFSException contains invalid fields: " + errorString);
            }
        }

        private WorkItem UpdateExisitingWorkItem(WorkItem wi, ExceptionEntity exception)
        {
            //string comment, string changeSet, WorkItem wi, string username
            // wi.Open();
            //if (wi.Links == null)
            //{
            //    ServiceLog.Warning($"Links is null for work item {wi.Id}. Work Item  will not be updated");
            //    return wi;
            //}

            if (IsWorkItemFixed(wi))
            {
                ServiceLog.Information($"Workitem {wi.Id} is fixed, update incident count, add comment ");
                UpdateCommentAndRefCount(exception.Comment, wi, exception.Username);
                return wi;
            }

            if (IsWorkItemClosedButNotFixed(wi))
            {
                ServiceLog.Information(
                    $"Workitem {wi.Id} is closed but not fixed, update incident count, add comment, reopen ");
                UpdateCommentAndRefCount(exception.Comment, wi, exception.Username);
                wi.State("Approved");
                return wi;
            }

            //If not limit reached increment count, and append comment.
            if (!IsLimitReached(wi))
            {
                ServiceLog.Information($"Updating existing Workitem {wi.Id}");
                UpdateCommentAndRefCount(exception.Comment, wi, exception.Username);
                UpdateBuildVersion(exception.Version, wi);
                UpdateApplication(exception.ApplicationName, wi);
            }
            else
            {
                ServiceLog.Information($"Workitem {wi.Id} is fixed, or has reached max # refCount ");
                //Set to null so we don't update as the workitem is either resolved
                //or max refcounts have been reached.
                wi = null;
            }

            return wi;
        }

        private void UpdateApplication(string applicationName, WorkItem wi)
        {
            if (string.IsNullOrEmpty(applicationName))
                return;
            var currentAppName = wi.Fields[Application] as string;
            currentAppName = applicationName;
        }

        private void UpdateBuildVersion(string version, WorkItem wi)
        {
            if (string.IsNullOrEmpty(version))
                return;

            var currentVersion = wi.Fields[BuildVersionFieldName] as string;
            currentVersion = version;
        }

        private bool IsWorkItemFixed(WorkItem wi)
        {
            return wi.State() == "Done";
            // if no changeSet specified for exception, set to 0
            //int.TryParse(exception.ChangeSet, out var changeSetId);

            //var vcs = tfs.GetService<VersionControlServer>();
            //if (vcs == null)
            //{
            //    return false;
            //}

            //var state = new ExceptionState(wi, vcs);

            ////Check if workitem is closed/resolved with later changeset than in exception
            //return !state.IsOpen && state.IsFixedAfterChangeset(changeSetId);
        }

        private bool IsWorkItemClosedButNotFixed(WorkItem wi)
        {
            return IsWorkItemFixed(wi);
        }
        //    var vcs = tfs.GetService<VersionControlServer>();
        //    if (vcs == null)
        //    {
        //        return false;
        //    }

        //    var state = new ExceptionState(wi, vcs);
        //    return !state.IsOpen && !state.HasChangeset();
        //}

        private bool HasHigherVersion(string buildVersion, WorkItem wi)
        {
            if (wi.Field(BuildVersionFieldName).Length == 0)
                return false;

            var wiVersion = wi.Field(BuildVersionFieldName);
            return CompareVersions(buildVersion, wiVersion);
        }

        private bool HasLowerVersion(string buildVersion, WorkItem wi)
        {
            if (wi.Field(BuildVersionFieldName).Length == 0)
                return false;

            var wiVersion = wi.Field(BuildVersionFieldName);
            return CompareVersions(wiVersion, buildVersion);
        }

        private bool CompareVersions(string ver1, string ver2)
        {
            if (string.IsNullOrEmpty(ver1)) ver1 = "0.0.0";
            if (string.IsNullOrEmpty(ver2)) ver2 = "0.0.0";

            var a = new Version(ver1);
            var b = new Version(ver2);
            return a > b;
        }

        private void UpdateCommentAndRefCount(string comment, WorkItem wi, string username)
        {
            var f = wi.Fields[RefCountFieldName] as string;
            var nRefCount = 0;
            if (f.Length > 0)
            {
                nRefCount = Convert.ToInt32(f);
            }

            nRefCount++;
            wi.Fields[RefCountFieldName] = nRefCount.ToString();

            var commentField = wi.Fields[CommentFieldName] as string;
            var sComments = string.Empty;
            if (commentField.Length > 0)
            {
                sComments = (string) commentField;
            }

            sComments += "\r\n" + username + ":\r\n" + comment;
            commentField = sComments;
            wi.Fields[CommentFieldName] = commentField;
        }

        private bool IsLimitReached(WorkItem wi)
        {
            var f = wi.Fields[RefCountFieldName] as string;
            var nRefCount = f.Length > 0 ? Convert.ToInt32(f) : 1;

            var sLimit = System.Configuration.ConfigurationManager.AppSettings["Limit"];

            return int.TryParse(sLimit, out var nLimit) && nRefCount < nLimit;
        }

        //private void ConnectToTfs(ExceptionEntity exception, IApplicationInfo applicationInfo)
        //{
        //    //new NetworkCredential(@"os-lab\oslabadmin", "Y67uJi)9");
        //    var credentials = CredentialCache.DefaultNetworkCredentials;//new NetworkCredential(@"partner\xNataliaA", "xNataliaA62");//

        //    tfs = new TfsTeamProjectCollection(new Uri(applicationInfo.TfsServer + "/" + applicationInfo.TeamProject), credentials);
        //    tfs.EnsureAuthenticated();
        //    if (!tfs.HasAuthenticated)
        //    {
        //        throw new ExceptionReporterException("Failed to authenticate to TFS.");
        //    }
        //}

        //private WorkItemType GetWorkItemType(ExceptionEntity exception, IApplicationInfo applicationInfo)
        //{
        //    var store = tfs.GetService<WorkItemStore>();
        //    var teamProject = store.Projects[applicationInfo.TeamProject];
        //    return teamProject.WorkItemTypes[ExceptionWorkItemType];
        //}

        //private void AddLinkedWorkItem(WorkItem linkedWi, WorkItem wi)
        //{
        //    try
        //    {
        //        var rl = new RelatedLink(linkedWi.Id);
        //        wi.Links.Add(rl);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ServiceLog.Warning(
        //            $"Failed to link work item {linkedWi.Id} while create new exception with stackTrace CRC {wi.Fields[StackChecksumFieldName].Value}" + ex.Message);
        //    }
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        // dispose managed resources
        //        tfs?.Dispose();
        //    }
        //    // free native resources
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        public ExceptionEntity GetWorkItem(ExceptionEntity exceptionEntity, ExceptionSettings applicationInfo)
        {
            var workItems = new ExceptionWorkItemCollection(exceptionEntity);
            var wi = workItems.OpenWorkItems.FirstOrDefault();

            if (wi == null)
                throw new System.Exception("Failed to find Exception Work item same stack trace");

            //   wi.Fields[AssignedToFieldName].Value = applicationInfo.AssignedTo;

            return new ExceptionEntity()
            {
                ApplicationName = wi.Field(Application),
                Reporter = wi.Field(ExceptionReporterFieldName),
                Version = wi.Field(BuildVersionFieldName),
                ExceptionMessage = wi.Field(ExceptionMessageExFieldName),
                ExceptionType = wi.Field(ExceptionTypeFieldName),
                TheClass = wi.Field(ClassFieldName),
                TheMethod = wi.Field(MethodFieldName),
                TheSource = wi.Field(SourceFieldName),
                StackTrace = wi.Field(StackTraceFieldName),
                Comment = wi.Field(CommentFieldName),
                ExceptionTitle = wi.Field("System.Title")
            };
        }
    }


    public class ExceptionSpecifics
    {
        public string RefCount { get; set; }
        public string ExceptionReporter { get; set; }
        public string BuildVersion { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionMessageEx { get; set; } = "";
        public string ExceptionType { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string StackChecksum { get; set; }
        public string AssemblyName { get; set; }


        public override string ToString()
        {
            var msg =
                $"Checksum:{StackChecksum}\r\n#Incidents:{RefCount}\r\n#Reported by: {ExceptionReporter}\r\n#ExceptionType:{ExceptionType}\r\n#Assembly:{AssemblyName}\r\n#Class:{Class}\r\n#Method:{Method}\r\n#Source:{Source}\r\n#BuildVersion:{BuildVersion}\r\n#ExceptionMessage:{ExceptionMessage}->->->{ExceptionMessageEx}\r\n#StackTrace:{StackTrace}";
            return msg;
        }


        public static ExceptionSpecifics CreateExceptionSpecifics(string msg)
        {
            var elements = msg.Split('#');
            var es = new ExceptionSpecifics();
            int i = 0;
            foreach (var element in elements)
            {
                var infos = element.Split(':');
                if (infos.Length != 2)
                {
                    i++;
                    continue;
                }

                var info = infos[1].Trim('\n').Trim('\r').Trim();
                switch (i)
                {
                    case 0:
                        es.StackChecksum = info;
                        break;
                    case 1:
                        es.RefCount = info;
                        break;
                    case 2:
                        es.ExceptionReporter = info;
                        break;
                    case 3:
                        es.ExceptionType = info;
                        break;
                    case 4:
                        es.AssemblyName = info;
                        break;
                    case 5:
                        es.Class = info;
                        break;
                    case 6:
                        es.Method = info;
                        break;
                    case 7:
                        es.Source = info;
                        break;
                    case 8:
                        es.BuildVersion = info;
                        break;
                    case 9:
                        es.ExceptionMessage = info;
                        break;
                    case 10:
                        es.StackTrace = info;
                        break;
                }


                i++;
            }

            return es;
        }

        public void IncrementIncidentCount()
        {
            var val = Convert.ToInt32(RefCount) + 1;
            RefCount = val.ToString();
        }


        //public class TFSStoreWithBug : IDisposable
        //{
        //    //Workitem-types:
        //    private const string ExceptionWorkItemType = "Bug";

        //    //Workitem-fields:
        // //   private const string Application = "Osiris.Application";
        //    private const string AssignedToFieldName = "System.AssignedTo";
        //    private const string Description = "System.Description";

        //    private const string ReproSteps = "Microsoft.VSTS.TCM.ReproSteps";

        //    private const string SystemInfo = "Microsoft.VSTS.TCM.SystemInfo";

        //    private const string AcceptanceCriteria = "Microsoft.VSTS.Common.AcceptanceCriteria";

        //    private const string FoundInBuild = "";
        // //   private const string RefCountFieldName = "Osiris.Exception.IncidentCount";
        // //   private const string ExceptionReporterFieldName = "Osiris.Exception.ExceptionReporter";
        //    //private const string BuildVersionFieldName = "Osiris.Exception.BuildVersion";
        //    //private const string ExceptionMessageFieldName = "Osiris.Exception.Message";
        //    //private const string ExceptionMessageExFieldName = "Osiris.Exception.MessageEx";
        //    //private const string ExceptionTypeFieldName = "Osiris.Exception.Type";
        //    //private const string ClassFieldName = "Osiris.Exception.ClassName";
        //    //private const string MethodFieldName = "Osiris.Exception.MethodName";
        //    //private const string SourceFieldName = "Osiris.Exception.Source";
        //    //private const string StackTraceFieldName = "Osiris.Exception.StackTrace";
        //    //private const string StackChecksumFieldName = "Osiris.Exception.StackChecksum";
        //    //private const string AssemblyName = "Inmeta.AssemblyName";


        //    public void RegisterException(ExceptionEntity exceptionEntity, IApplicationInfo applicationInfo)
        //    {

        //        ConnectToTfs(exceptionEntity, applicationInfo);

        //        WorkItem wi = null;
        //        WorkItem linkedWi = null;
        //        var workItems = new ExceptionWorkItemCollection(exceptionEntity);

        //        if (!workItems.HasOpenWorkItems)
        //        {
        //            if (workItems.HasWorkItemsWithHigherChangeset)
        //            {
        //                wi = workItems.GetWorkItemWithHigherChangeset();
        //                // If not Open and new exception has higher version, we should create new wi with link to the old one
        //                if (HasHigherVersion(exceptionEntity.Version, wi))
        //                {
        //                    linkedWi = wi;
        //                    wi = null;
        //                }
        //            }
        //            else
        //            {
        //                wi = workItems.GetLatestNotOpenWorkItem();
        //            }
        //        }
        //        else
        //        {
        //            wi = workItems.OpenWorkItems.FirstOrDefault();
        //            if (wi != null && HasLowerVersion(exceptionEntity.Version, wi))
        //            {
        //                ServiceLog.Information(
        //                    $"Exception with newer version already exists with ID={wi.Id}. Coming exception will be dropped.");
        //                return;
        //            }
        //        }

        //        wi = wi != null
        //                 ? UpdateExisitingWorkItem(wi, exceptionEntity)
        //                 : CreateNewException(exceptionEntity, applicationInfo, linkedWi);

        //        if (wi == null)
        //            return;

        //        var errors = wi.Validate();
        //        if (errors.Count == 0)
        //        {
        //            wi.Save();
        //        }
        //        else
        //        {
        //            var errorString = new StringBuilder();
        //            foreach (Field f in errors)
        //                errorString.AppendLine($"Field '{f.Name}' has invalid value = '{wi[f.ReferenceName]}'");
        //            ServiceLog.Warning("Workitem failed validation: " + errorString);

        //            //LARS: should throw exception when exception is not valid:
        //            throw new ArgumentException("TFSException contains invalid fields: " + errorString);
        //        }
        //    }

        //    private WorkItem UpdateExisitingWorkItem(WorkItem wi, ExceptionEntity exception)
        //    {

        //        //string comment, string changeSet, WorkItem wi, string username
        //        wi.Open();
        //        if (wi.Links == null)
        //        {
        //            ServiceLog.Warning($"Links is null for work item {wi.Id}. Work Item  will not be updated");
        //            return wi;
        //        }

        //        if (IsWorkItemFixed(wi, exception))
        //        {
        //            ServiceLog.Information($"Workitem {wi.Id} is fixed, update incident count, add comment ");
        //            UpdateCommentAndRefCount(exception.Comment, wi, exception.Username);
        //            return wi;
        //        }

        //        if (IsWorkItemClosedButNotFixed(wi))
        //        {
        //            ServiceLog.Information(
        //                $"Workitem {wi.Id} is closed but not fixed, update incident count, add comment, reopen ");
        //            UpdateCommentAndRefCount(exception.Comment, wi, exception.Username);
        //            wi.State = "Active";
        //            return wi;
        //        }

        //        //If not limit reached increment count, and append comment.
        //        if (!IsLimitReached(wi))
        //        {
        //            ServiceLog.Information($"Updating existing Workitem {wi.Id}");
        //            UpdateCommentAndRefCount(exception.Comment, wi, exception.Username);
        //            UpdateBuildVersion(exception.Version, wi);
        //            UpdateApplication(exception.ApplicationName, wi);
        //        }
        //        else
        //        {
        //            ServiceLog.Information($"Workitem {wi.Id} is fixed, or has reached max # refCount ");
        //            //Set to null so we don't update as the workitem is either resolved
        //            //or max refcounts have been reached.
        //            wi = null;
        //        }
        //        return wi;
        //    }

        //    private void UpdateApplication(string applicationName, WorkItem wi)
        //    {
        //        if (string.IsNullOrEmpty(applicationName))
        //            return;
        //        var currentAppName = wi.Fields[Description/*Application*/];
        //        currentAppName.Value = applicationName;
        //    }

        //    private void UpdateBuildVersion(string version, WorkItem wi)
        //    {
        //        if (string.IsNullOrEmpty(version))
        //            return;

        //        var currentVersion = wi.Fields[Description /*BuildVersionFieldName*/];
        //        currentVersion.Value = version;
        //    }

        //    private bool IsWorkItemFixed(WorkItem wi, ExceptionEntity exception)
        //    {

        //        // if no changeSet specified for exception, set to 0
        //        int.TryParse(exception.ChangeSet, out var changeSetId);

        //        var vcs = tfs.GetService<VersionControlServer>();
        //        if (vcs == null)
        //        {
        //            return false;
        //        }

        //        var state = new ExceptionState(wi, vcs);

        //        //Check if workitem is closed/resolved with later changeset than in exception
        //        return !state.IsOpen && state.IsFixedAfterChangeset(changeSetId);
        //    }

        //    private bool IsWorkItemClosedButNotFixed(WorkItem wi)
        //    {
        //        var vcs = tfs.GetService<VersionControlServer>();
        //        if (vcs == null)
        //        {
        //            return false;
        //        }

        //        var state = new ExceptionState(wi, vcs);
        //        return !state.IsOpen && !state.HasChangeset();
        //    }

        //    private bool HasHigherVersion(string buildVersion, WorkItem wi)
        //    {
        //        //if (!wi.Fields.Contains(BuildVersionFieldName))
        //        //    return false;

        //        //var wiVersion = wi.Fields[BuildVersionFieldName].Value.ToString();
        //        //return CompareVersions(buildVersion, wiVersion);
        //        return false;
        //    }

        //    private bool HasLowerVersion(string buildVersion, WorkItem wi)
        //    {
        //        //if (!wi.Fields.Contains(BuildVersionFieldName))
        //        //    return false;

        //        //var wiVersion = wi.Fields[BuildVersionFieldName].Value.ToString();
        //        //return CompareVersions(wiVersion, buildVersion);
        //        return false;
        //    }

        //    private bool CompareVersions(string ver1, string ver2)
        //    {
        //        if (string.IsNullOrEmpty(ver1)) ver1 = "0.0.0";
        //        if (string.IsNullOrEmpty(ver2)) ver2 = "0.0.0";

        //        var a = new Version(ver1);
        //        var b = new Version(ver2);
        //        return a > b;
        //    }

        //    private void UpdateCommentAndRefCount(string comment, WorkItem wi, string username)
        //    {
        //        //var f = wi.Fields[RefCountFieldName];
        //        //var nRefCount = 0;
        //        //if (f.Value != null)
        //        //{
        //        //    nRefCount = (int)f.Value;
        //        //}
        //        //nRefCount++;
        //        //f.Value = nRefCount;

        //        var commentField = wi.Fields[Description];
        //        var sComments = string.Empty;
        //        if (commentField.Value != null)
        //        {
        //            sComments = (string)commentField.Value;
        //        }
        //        sComments += "\r\n" + username + ":\r\n" + comment;
        //        commentField.Value = sComments;


        //    }

        //    private bool IsLimitReached(WorkItem wi)
        //    {
        //        var f = wi.Fields[Description/*RefCountFieldName*/];
        //        var nRefCount = f != null && f.Value != null ? (int)f.Value : 0;

        //        var sLimit = System.Configuration.ConfigurationManager.AppSettings["Limit"];
        //        int nLimit; //= m_nLimit; TODO: Commented out because it has no effect. int.TryParse() sets the out-parameter to 0 if parsing fails. If this method was commented, i could have fixed it instead...

        //        return int.TryParse(sLimit, out nLimit) && nRefCount < nLimit;
        //    }

        //    private void ConnectToTfs(ExceptionEntity exception, IApplicationInfo applicationInfo)
        //    {
        //        //new NetworkCredential(@"os-lab\oslabadmin", "Y67uJi)9");
        //        var credentials = CredentialCache.DefaultNetworkCredentials;//new NetworkCredential(@"partner\xNataliaA", "xNataliaA62");//

        //        tfs = new TfsTeamProjectCollection(new Uri(applicationInfo.TfsServer + "/" + applicationInfo.TeamProject), credentials);
        //        tfs.EnsureAuthenticated();
        //        if (!tfs.HasAuthenticated)
        //        {
        //            throw new ExceptionReporterException("Failed to authenticate to TFS.");
        //        }
        //    }

        //    private WorkItemType GetWorkItemType(ExceptionEntity exception, IApplicationInfo applicationInfo)
        //    {
        //        var store = tfs.GetService<WorkItemStore>();
        //        var teamProject = store.Projects[applicationInfo.TeamProject];
        //        return teamProject.WorkItemTypes[ExceptionWorkItemType];
        //    }

        //    private WorkItem CreateNewException(ExceptionEntity exception, IApplicationInfo applicationInfo, WorkItem linkedWi)
        //    {
        //        //ensure no problem with string NG 255

        //        var wi = new WorkItem(GetWorkItemType(exception, applicationInfo))
        //        {
        //            Title = TFSStringUtil.GenerateValidTFSStringType(exception.ExceptionTitle),
        //            Description = exception.Username + ":\n" + exception.Comment,
        //            AreaPath = applicationInfo.Area
        //        };

        //        //wi.Fields[Application].Value = exception.ApplicationName;
        //        //wi.Fields[AssignedToFieldName].Value = applicationInfo.AssignedTo;
        //        //wi.Fields[ExceptionReporterFieldName].Value = exception.Reporter;
        //        //wi.Fields[BuildVersionFieldName].Value = exception.Version;

        //        //if (wi.Fields.Contains(ExceptionMessageExFieldName))
        //        //    wi.Fields[ExceptionMessageExFieldName].Value = exception.ExceptionMessage;

        //        //wi.Fields[ExceptionMessageFieldName].Value = TFSStringUtil.GenerateValidTFSStringType(exception.ExceptionMessage);
        //        //wi.Fields[ExceptionTypeFieldName].Value = exception.ExceptionType;

        //        //var kmParams = exception.TheClass.Split('|');
        //        //wi.Fields[ClassFieldName].Value = kmParams[0];

        //        //if (wi.Fields.Contains(AssemblyName) && kmParams.Count() > 1)
        //        //{
        //        //    wi.Fields[AssemblyName].Value = kmParams[1];
        //        //}
        //        //wi.Fields[MethodFieldName].Value = exception.TheMethod;
        //        //wi.Fields[SourceFieldName].Value = exception.TheSource;
        //        //wi.Fields[StackTraceFieldName].Value = exception.StackTrace;
        //        //wi.Fields[StackChecksumFieldName].Value = (int)Crc32.GetStreamCrc32(exception.StackTrace);

        //        if (linkedWi != null)
        //        {
        //            AddLinkedWorkItem(linkedWi, wi);
        //        }

        //        return wi;
        //    }

        //    private void AddLinkedWorkItem(WorkItem linkedWi, WorkItem wi)
        //    {
        //        try
        //        {
        //            var rl = new RelatedLink(linkedWi.Id);
        //            wi.Links.Add(rl);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            //ServiceLog.Warning(
        //            //    $"Failed to link work item {linkedWi.Id} while create new exception with stackTrace CRC {wi.Fields[StackChecksumFieldName].Value}" + ex.Message);
        //        }
        //    }

        //    protected virtual void Dispose(bool disposing)
        //    {
        //        if (disposing)
        //        {
        //            // dispose managed resources
        //            tfs?.Dispose();
        //        }
        //        // free native resources
        //    }

        //    public void Dispose()
        //    {
        //        Dispose(true);
        //        GC.SuppressFinalize(this);
        //    }

        //public ExceptionEntity GetWorkItem(ExceptionEntity exceptionEntity, ExceptionSettings applicationInfo)
        //{
        //    var workItems = new ExceptionWorkItemCollection(exceptionEntity);
        //    var wi = workItems.OpenWorkItems.FirstOrDefault();

        //    if (wi == null)
        //        throw new System.Exception("Failed to find Exception Work item same stack trace");

        //    //   wi.Fields[AssignedToFieldName].Value = applicationInfo.AssignedTo;

        //    return new ExceptionEntity()
        //    {
        //        ApplicationName = wi.Fields(Application).Value.ToString(),
        //        Reporter = wi.Fields[ExceptionReporterFieldName].Value.ToString(),
        //        Version = wi.Fields[BuildVersionFieldName].Value.ToString(),
        //        ExceptionMessage = wi.Fields[ExceptionMessageExFieldName].Value.ToString(),
        //        ExceptionType = wi.Fields[ExceptionTypeFieldName].Value.ToString(),
        //        TheClass = wi.Fields[ClassFieldName].Value.ToString(),
        //        TheMethod = wi.Fields[MethodFieldName].Value.ToString(),
        //        TheSource = wi.Fields[SourceFieldName].Value.ToString(),
        //        StackTrace = wi.Fields[StackTraceFieldName].Value.ToString(),
        //        Comment = wi.Fields[Description].Value.ToString(),
        //        //ExceptionTitle = wi.Title
        //    };
        //}
    }
}