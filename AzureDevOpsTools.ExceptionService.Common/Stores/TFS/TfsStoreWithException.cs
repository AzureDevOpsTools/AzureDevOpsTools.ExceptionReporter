using System;
using System.Linq;
using System.Text;
using AzureDevOpsTools.Exception.Common;
using AzureDevOpsTools.ExceptionService.Common;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AzureDevOpsTools.Exception.Common.Stores.TFS
{
    public class TfsStoreWithException : AccessToVsts
    {
        //Workitem-types:
        private const string ExceptionWorkItemType = "Exception";

        //Workitem-fields:
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
                ExceptionMessage = wi.Field(ExceptionMessageFieldName),
                ExceptionType = wi.Field(ExceptionTypeFieldName),
                ExceptionClass = wi.Field(ClassFieldName),
                ExceptionMethod = wi.Field(MethodFieldName),
                ExceptionSource = wi.Field(SourceFieldName),
                StackTrace = wi.Field(StackTraceFieldName),
                Comment = wi.Field(CommentFieldName),
                ExceptionTitle = wi.Field("System.Title")
            };
        }
    }
}