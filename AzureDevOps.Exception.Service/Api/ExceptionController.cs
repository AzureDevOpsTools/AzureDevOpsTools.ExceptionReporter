using AzureDevOps.Exception.Service.Common;
using AzureDevOps.Exception.Service.Common.Stores.TFS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOps.Exception.Service.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        // POST: api/Exception
        [HttpPost]
        public void Post([FromBody] ExceptionEntity exception)
        {
            SendToStore(exception);

        }

        [HttpGet]
        public string Get()
        {
            return "Hello";
        }

        private void SendToStore(ExceptionEntity exception)
        {
            try
            {
                //if (_storeIsTFS)
                    StoreInTFS(exception);

                //StoreInFile(exceptionEntity);
            }
            catch (System.Exception ex)
            {
                //ServiceLog.Error("Error adding new Exception: " + ex.ToString());
                throw;
            }
        }

        private void StoreInTFS(ExceptionEntity exception)
        {
            var registrator = new TfsStoreWithException();
            var settings = new ExceptionSettings(exception.ApplicationName);
            registrator.RegisterException(exception, settings);
        }


    }
}
