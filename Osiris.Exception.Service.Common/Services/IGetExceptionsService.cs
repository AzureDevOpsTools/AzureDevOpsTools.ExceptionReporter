using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Inmeta.Exception.Service.Common.Services
{
    /// <summary>
    /// This service provides functionality for getting all exceptions stored locally.
    /// It is not part of the Service.asmx since this is a REST service and requires a seperate security modell.
    /// </summary>
    [ServiceContract]
    public interface IGetExceptionsService
    {
        [WebGet(UriTemplate = "/", ResponseFormat = WebMessageFormat.Json)]
        IList<ExceptionEntity> GetExceptions();

        [OperationContract]
        [WebGet(UriTemplate = "/rel", ResponseFormat = WebMessageFormat.Json)]
        KeyValuePair<string, IEnumerable<ExceptionEntity>> GetExceptionsReliable();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "/ack={key}")]
        bool AckDelivery(string key);
    }
}
