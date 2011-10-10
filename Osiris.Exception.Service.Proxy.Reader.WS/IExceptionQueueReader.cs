using System.ServiceModel;
using Osiris.Exception.Service.Common;

namespace Osiris.Exception.Service.Proxy.Reader.WS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IExceptionQueueReader
    {
        [OperationContract]
        ExceptionEntity GetException();
    }
}
