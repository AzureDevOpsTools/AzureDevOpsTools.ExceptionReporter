using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AzureDevOps.Exception.Common;
using AzureDevOps.Exception.Service.Common.Sec;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AzureDevOps.Exception.Service.Common.Stores.TFS
{
    public class ExceptionWorkItemCollection  : AccessToVsts
    {
        private IEnumerable<WorkItem> workItems;
        private readonly ExceptionEntity exception;
    
        private string TeamProject
        {
            get;
        }

       
        public ExceptionWorkItemCollection(ExceptionEntity exceptionEntity)
        {
            TeamProject = Project;
            exception = exceptionEntity;

            workItems = new List<WorkItem>();
            //First check to see if an exception with the same stacktrace exists.
            SearchForStackTrace();
        }

        /// <summary>
        /// All workitems are closed, check if any of them has solved the issue with a higher
        /// changeset number.).
        /// </summary>
        internal bool HasWorkItemsWithHigherChangeset => FindWorkItemsWithHigherChangeSet().Any();

        /// <summary>
        /// get the latest work item of all registered
        /// </summary>
        /// <returns></returns>
        internal WorkItem GetWorkItemWithHigherChangeset()
        {
            var res = FindWorkItemsWithHigherChangeSet().Aggregate((wi, x) => x.Id > wi.Id ? x : wi);
            return res;
        }

        /// <summary>
        /// get the latest work item of all registered
        /// </summary>
        /// <returns></returns>
        internal WorkItem GetLatestNotOpenWorkItem()
        {
            return workItems.Where(IsNotOpen).Any() ? workItems.Where(IsNotOpen).Aggregate((wi, x) => x.Id > wi.Id ? x : wi) : null;
        }

        /// <summary>
        /// A open workitem is one that is not Resolved or Closed
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<WorkItem> OpenWorkItems => workItems.Where(IsOpen);

        internal bool HasOpenWorkItems => OpenWorkItems.Any();

    

        private void SearchForStackTrace()
        {
            var parameters = new Hashtable { { "project", TeamProject } };
            var crc = (int)Crc32.GetStreamCrc32(exception.StackTrace);
            parameters.Add("checksum", crc);

            const string query = "SELECT [System.ID],[Stack Trace], [System.Title] from WorkItems where [System.TeamProject] = @project AND "
                                 + "[System.WorkItemType]='Exception' and [StackChecksum] = @checksum";
            var wiql = new Wiql {Query = query};
            using (var workItemTrackingHttpClient = new WorkItemTrackingHttpClient(Uri, Credentials))
            {
                var items = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;
                if (items == null || !items.WorkItems.Any())
                {
                    ServiceLog.Error("WorkItemStore unexpectedly returned null for query: " + query);
                    return;
                }

                workItems = items.WorkItems.Cast<WorkItem>();
            }
        }

        private IEnumerable<WorkItem> FindWorkItemsWithHigherChangeSet()
        {
            return workItems.Where(wi => IsExceptionFixed(wi, exception.ChangeSet));
        }

        private bool IsExceptionFixed(WorkItem wi, string sChangeSet)
        {
            int.TryParse(sChangeSet, out var changeSetId);
            var state = new ExceptionState(wi);
            return false;
            //return state.IsFixedAfterChangeset(changeSetId);
        }

        private bool IsOpen(WorkItem wi)
        {
            var state = new ExceptionState(wi);
            return state.IsOpen;
        }

        private bool IsNotOpen(WorkItem wi)
        {
            var state = new ExceptionState(wi);
            return !state.IsOpen;
        }
     
    }
}