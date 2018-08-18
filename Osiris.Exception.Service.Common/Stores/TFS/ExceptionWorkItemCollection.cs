using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Inmeta.Exception.Service.Common.Sec;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Inmeta.Exception.Service.Common.Stores.TFS
{

    public class ExceptionWorkItemCollection
    {
        private IEnumerable<WorkItem> workItems;
        private readonly ExceptionEntity exception;
        private readonly WorkItemStore workItemStore;
        private readonly VersionControlServer versionControlServer;

        private string TeamProject
        {
            get;
        }

       
        internal ExceptionWorkItemCollection(string teamProject, WorkItemStore wis, VersionControlServer vcs, ExceptionEntity exceptionEntity)
        {
            TeamProject = teamProject;
            workItemStore = wis ?? throw new ArgumentNullException(nameof(wis));
            versionControlServer = vcs ?? throw new ArgumentNullException(nameof(vcs));
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
            var items = workItemStore.Query(query, parameters);

            if (items == null)
            {
                throw new ExceptionReporterException("WorkItemStore unexpectedly returned null for query: " + query);
            }

            workItems = items.Cast<WorkItem>();
        }

        private IEnumerable<WorkItem> FindWorkItemsWithHigherChangeSet()
        {
            return workItems.Where(wi => IsExceptionFixed(wi, exception.ChangeSet));
        }

        private bool IsExceptionFixed(WorkItem wi, string sChangeSet)
        {
            int.TryParse(sChangeSet, out var changeSetId);
            var state = new ExceptionState(wi, versionControlServer);
            return state.IsFixedAfterChangeset(changeSetId);
        }

        private bool IsOpen(WorkItem wi)
        {
            var state = new ExceptionState(wi, versionControlServer);
            return state.IsOpen;
        }

        private bool IsNotOpen(WorkItem wi)
        {
            var state = new ExceptionState(wi, versionControlServer);
            return !state.IsOpen;
        }
     
    }
}