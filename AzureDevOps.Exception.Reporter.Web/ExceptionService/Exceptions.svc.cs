using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using AzureDevOps.Exception.Service.Common;
using AzureDevOps.Exception.Service.Common.Services;
using AzureDevOps.Exception.Service.Common.Stores.FileStore;

namespace AzureDevOps.Exception.Reporter.Web.ExceptionService
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
