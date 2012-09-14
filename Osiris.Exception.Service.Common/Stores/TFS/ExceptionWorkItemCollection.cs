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
            set;
        }
        [ContractInvariantMethod]
// ReSharper disable UnusedMember.Local
        private void Invariants()
// ReSharper restore UnusedMember.Local
        {
            Contract.Invariant(workItemStore != null);
            Contract.Invariant(versionControlServer != null);
            Contract.Invariant(exception != null);
            Contract.Invariant(workItems != null);
        }

        internal ExceptionWorkItemCollection(string teamProject, WorkItemStore wis, VersionControlServer vcs, ExceptionEntity exceptionEntity)
        {
            Contract.Requires(teamProject != null);
            Contract.Requires(exceptionEntity != null);
            Contract.Requires(exceptionEntity.StackTrace != null);
            Contract.Ensures(workItems != null);

            if (wis == null)
            {
                throw new ArgumentNullException("wis");
            }
            
            if (vcs == null)
            {
                throw new ArgumentNullException("vcs");
            }

            TeamProject = teamProject;
            workItemStore = wis;
            versionControlServer = vcs;
            exception = exceptionEntity;

            workItems = new List<WorkItem>();
            //First check to see if an exception with the same stacktrace exists.
            SearchForStackTrace();
        }

        internal bool HasWorkItemsWithHigherChangeset
        {
            get
            {                
                //All workitem are closed, check if any of them has solved the issue with a higher
                //changeset number.).
                return FindWorkItemsWithHigherChangeSet().Any();
                //The fix will be arriving in a later release, so ignore the exception for now.
            }
        }

        internal WorkItem GetWorkItemWithHigherChangeset()
        {            
            // get the latest work item of all registered
            var res = FindWorkItemsWithHigherChangeSet().Aggregate((wi, x) => ((x.Id > wi.Id) ? x : wi));
            return res;
        }

        /// <summary>
        /// A open workitem is one that is not Resolved or Closed
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<WorkItem> OpenWorkItems
        {
            get 
            {
                return workItems.Where(IsOpen);
            }
        }

        internal bool HasOpenWorkItems
        {
            get
            {
                return OpenWorkItems.Any();
            }
        }

        #region Private

        private void SearchForStackTrace()
        {
            Contract.Ensures(workItems != null);
           
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
            Contract.Ensures(Contract.Result<IEnumerable<WorkItem>>() != null);

            return workItems.Where(wi => IsExceptionFixed(wi, exception.ChangeSet));
        }

        private bool IsExceptionFixed(WorkItem wi, string sChangeSet)
        {
            Contract.Requires(wi != null);
            Contract.Requires(wi.Links != null);

            int changeSetId; //= int.MaxValue; TODO: Commented out because it has no effect. int.TryParse() sets the out-parameter to 0 if parsing fails. If this method was commented, i could have fixed it instead...
            int.TryParse(sChangeSet, out changeSetId);

            var state = new ExceptionState(wi, versionControlServer);
            return state.IsFixedAfterChangeset(changeSetId);            
        }

        private bool IsOpen(WorkItem wi)
        {
            Contract.Requires(wi != null);
            Contract.Requires(wi.Links != null);

            var state = new ExceptionState(wi, versionControlServer);
            return state.IsOpen;
        }
        #endregion
    }
}