//using System;
//using System.Collections.Generic;
//using System.Linq;
//using AzureDevOpsTools.Exception.Common;
//using AzureDevOpsTools.Exception.Common.Stores.TFS;
//using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;

//namespace AzureDevOpsTools.ExceptionService.Common.Stores
//{
//    public class ExceptionStore
//    {
//        private bool StoreIsTfs { get; set;}
//        private Uri ServerForwardingService { get; set; }

//        public ExceptionStore(Uri serverForwardingService = null, bool useTfs = true)
//        {
//            ServerForwardingService = serverForwardingService;
//            //default true
//            StoreIsTfs = useTfs;
//        }

//        public void StoreException(List<ExceptionEntity> exceptions, string applicationLocation)
//        {
//            //iterate over all exceptions and store. We need to create a new application settings each time since application name might change. 
//            exceptions.ToList().ForEach((exception) => StoreException(exception, new ExceptionSettings(exception.ApplicationName)));
//        }

//        public void StoreException(ExceptionEntity exp, IApplicationInfo settings)
//        {
//            //Lars TODO: all these stores should have been IOC injected... this function has to much logic...  
//#if OLD
//            //allways store in LOCAL FILE STORE
//            try
//            {
//                new FileStore.FileStore().SaveException(exp);
//            }
//            catch (System.Exception ex)
//            {
//               // ServiceLog.DefaultLog.Error("Failed to save exception to local file.", ex);
//            }
//#endif
//            StoreIsTfs = true;
//            //STORE IN TFS
//            if (StoreIsTfs)
//            {
//                try
//                {
//                    var registrator = new TfsStoreWithException();
                    
//                    registrator.RegisterException(exp, settings);
                    
//                }
//                catch (System.Exception ex)
//                {
//                    ServiceLog.Error($"Failed to register Exception in TFS: {ex}");
//                    throw;
//                }
//            }

//            return;
//            //forward to server.
//            //try
//            //{
//            //    new ForwardStore.ForwardStore(ServerForwardingService).Forward(exp);
//            //}
//            //catch (System.Exception ex)
//            //{
//            //    ServiceLog.Error("Failed to forward exception to : " + ServerForwardingService.ToString(), ex);
//            //}
//        }
//    }
//}
