//using System;

//namespace AzureDevOps.Exception.Service.Common.Stores.ForwardStore
//{
//    public class ForwardStore
//    {
//        private Uri _serverForwardingService { get; set; }

//        public ForwardStore(Uri serverForwardingService)
//        {
//            _serverForwardingService = serverForwardingService;
//        }

//        public void Forward(ExceptionEntity ex)
//        {
//            if (_serverForwardingService != null)
//                Channel.AddNewApplicationException(ex);
//        }

//        private static readonly object _synch = new object();
//        private static volatile WebServiceClient<IExceptionService> _client;

//        private IExceptionService Channel
//        {
//            get
//            {
//                if (_client == null)
//                {
//                    lock (_synch)
//                    {
//                        if (_client == null)
//                        {
//                            //UriBuilder build = new UriBuilder(System.IO.Path.Combine(ServiceSettings.ServiceUrl, "Service.asmx"));
//                            _client = new WebServiceClient<IExceptionService>(_serverForwardingService.ToString());
//                        }
//                    }
//                }
//                return _client.Channel;
//            }
//        }
//    }
//}
