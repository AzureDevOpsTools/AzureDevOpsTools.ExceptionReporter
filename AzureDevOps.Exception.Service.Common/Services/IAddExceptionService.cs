using System.Diagnostics.Contracts;
using System.ServiceModel;

namespace AzureDevOps.Exception.Service.Common.Services
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
             Action = "http://exceptions.maritimesim.com/AddNewException"
             ),
        DataContractFormat(Style = OperationFormatStyle.Document)]
        void AddNewException(string teamProject, string reporter, string comment, string version,
                             string exceptionMessage, string exceptionType, string exceptionTitle, string stackTrace,
                             string theClass, string theMethod, string theSource, string changeSet, string username);

        [OperationContract(
             Action = "http://exceptions.maritimesim.com/AddNewApplicationException"
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
                           
            }

            void IExceptionService.AddNewApplicationException(ExceptionEntity exceptionEntity)
            {
                
            }
        }
    }
}
