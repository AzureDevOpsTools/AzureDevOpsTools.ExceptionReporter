using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Inmeta.Exception.Service.Common.Services
{
    /// <summary>
    /// Think twice and even thrice before you change this interface.
    /// All applications with exception reporting deployed today rely on the web-service having this interface.
    /// </summary>
    [ContractClass(typeof(Contracts.ExceptionServiceContracts))]
    [ServiceContract, DataContractFormat(Style = OperationFormatStyle.Document)]
    public interface IExceptionService
    {
        [OperationContract(
             Action = "http://tempuri.org/AddNewException"
             ),
        DataContractFormat(Style = OperationFormatStyle.Document)]
        void AddNewException(string teamProject, string reporter, string comment, string version,
                             string exceptionMessage, string exceptionType, string exceptionTitle, string stackTrace,
                             string theClass, string theMethod, string theSource, string changeSet, string username);

        [OperationContract(
             Action = "http://tempuri.org/AddNewApplicationException"
             ),
        DataContractFormat(Style = OperationFormatStyle.Document)]
        void AddNewApplicationException(ExceptionEntity exceptionEntity);
    }

    namespace Contracts
    {
        [ContractClassFor(typeof(IExceptionService))]
        internal abstract class ExceptionServiceContracts : IExceptionService
        {
            void IExceptionService.AddNewException(string teamProject, string reporter, string comment, string version, string exceptionMessage, string exceptionType, string exceptionTitle, string stackTrace, string theClass, string theMethod, string theSource, string changeSet, string username)
            {
                Contract.Requires(teamProject != null);
                Contract.Requires(reporter != null);
                Contract.Requires(comment != null);
                Contract.Requires(version != null);
                Contract.Requires(exceptionMessage != null);
                Contract.Requires(exceptionType != null);
                Contract.Requires(exceptionTitle != null);
                Contract.Requires(stackTrace != null);
                Contract.Requires(theClass != null);
                Contract.Requires(theMethod != null);
                Contract.Requires(theSource != null);
                Contract.Requires(changeSet != null);
                Contract.Requires(username != null);                
            }

            void IExceptionService.AddNewApplicationException(ExceptionEntity exceptionEntity)
            {
                Contract.Requires(exceptionEntity != null);
                Contract.Requires(exceptionEntity.ApplicationName != null);
                Contract.Requires(exceptionEntity.Reporter != null);
                Contract.Requires(exceptionEntity.Comment != null);
                Contract.Requires(exceptionEntity.Version != null);
                Contract.Requires(exceptionEntity.ExceptionMessage != null);
                Contract.Requires(exceptionEntity.ExceptionType != null);
                Contract.Requires(exceptionEntity.ExceptionTitle != null);
                Contract.Requires(exceptionEntity.StackTrace != null);
                Contract.Requires(exceptionEntity.TheClass != null);
                Contract.Requires(exceptionEntity.TheMethod != null);
                Contract.Requires(exceptionEntity.TheSource != null);
                Contract.Requires(exceptionEntity.ChangeSet != null);
                Contract.Requires(exceptionEntity.Username != null);
            }
        }
    }
}
