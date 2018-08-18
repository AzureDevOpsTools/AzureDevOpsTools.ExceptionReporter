using System;
using System.Collections.Generic;
using System.Linq;
using Inmeta.Exception.Common;
using Inmeta.Exception.Service.Common.Stores.TFS;

namespace Inmeta.Exception.Service.Common.Stores
{
    public class ExceptionStore
    {
        private bool _storeIsTFS { get; set;}
        private Uri _serverForwardingService { get; set; }

        public ExceptionStore(Uri serverForwardingService = null, bool useTFS = true)
        {
            _serverForwardingService = serverForwardingService;
            //default true
            _storeIsTFS = useTFS;
        }

        public void StoreException(List<ExceptionEntity> exceptions, string applicationLocation)
        {
            //iterate over all exceptions and store. We need to create a new application settings each time since application name might change. 
            exceptions.ToList().ForEach((exception) => StoreException(exception, new ExceptionSettings(exception.ApplicationName, applicationLocation)));
        }

        public void StoreException(ExceptionEntity exp, IApplicationInfo settings)
        {
            //Lars TODO: all these stores should have been IOC injected... this function has to much logic...  
#if OLD
            //allways store in LOCAL FILE STORE
            try
            {
                new FileStore.FileStore().SaveException(exp);
            }
            catch (System.Exception ex)
            {
               // ServiceLog.DefaultLog.Error("Failed to save exception to local file.", ex);
            }
#endif
            _storeIsTFS = true;
            //STORE IN TFS
            if (_storeIsTFS)
            {
                try
                {
                    using (var registrator = new TFSStoreWithBug())
                    {
                        registrator.RegisterException(exp, settings);
                    }
                }
                catch (System.Exception ex)
                {
                    ServiceLog.DefaultLog.Error("Failed to register Exception in TFS: ", ex);
                    throw;
                }
            }

            return;
            //forward to server.
            try
            {
                new ForwardStore.ForwardStore(_serverForwardingService).Forward(exp);
            }
            catch (System.Exception ex)
            {
                ServiceLog.DefaultLog.Error("Failed to forward exception to : " + _serverForwardingService.ToString(), ex);
            }
        }
    }
}
