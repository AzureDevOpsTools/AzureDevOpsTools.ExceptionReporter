using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Services;
using Inmeta.Exception.Service.Common.Stores.FileStore;

namespace Inmeta.Exception.Reporter.Web.ExceptionService
{
   
    /// <summary>
    /// This service provides the endpoint for getting all exceptions stored locally.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Exceptions : IGetExceptionsService
    {
        private object _fileLockObject = new object();

        public IList<ExceptionEntity> GetExceptions()
        {
            lock (_fileLockObject)
            {
                return new FileStore().PopExceptions();
            }
        }

        public KeyValuePair<string, IEnumerable<ExceptionEntity>> GetExceptionsReliable()
        {
            lock (_fileLockObject)
            {
                string key = Guid.NewGuid().ToString();
                var res = new KeyValuePair<string, IEnumerable<ExceptionEntity>>(key, new FileStore().PopExceptionsWaitAck(key));
                return res;
            }
        }

        public bool AckDelivery(string key)
        {
            lock (_fileLockObject)
            {
                return new FileStore().Ack(key);
            }
        }
    }
}
