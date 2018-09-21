using System.Diagnostics.Contracts;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    [ContractClass(typeof(Contracts.ApplicationInfoContracts))]
    public interface IApplicationInfo
    {
        string ApplicationName { get; }
        string TfsServer { get; }
        string Collection { get; }
        string TeamProject { get; }
        string Area { get; }
        string AssignedTo { get; }
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IApplicationInfo))]
        internal abstract class ApplicationInfoContracts : IApplicationInfo
        {
            string IApplicationInfo.ApplicationName
            {
                get { return string.Empty; }
            }

            string IApplicationInfo.TfsServer
            {
                get { return string.Empty; }
            }

            string IApplicationInfo.Collection
            {
                get { return string.Empty; }
            }

            string IApplicationInfo.TeamProject
            {
                get
                {
                    Contract.Ensures(Contract.Result<string>() != null);
                    return string.Empty;
                }
            }

            string IApplicationInfo.Area
            {
                get { return string.Empty; }
            }

            string IApplicationInfo.AssignedTo
            {
                get { return string.Empty; }
            }
        }
    }
}
