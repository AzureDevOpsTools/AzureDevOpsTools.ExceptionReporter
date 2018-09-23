using AzureDevOpsTools.Exception.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Common;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOpsTools.ExceptionService.Web
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        // POST: api/Exception
        [HttpPost]
        public void Post([FromBody] ExceptionEntity exception)
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

        [HttpGet]
        public string Get()
        {
            return "Hello World!";
        }

        private void StoreInTFS(ExceptionEntity exception)
        {
            var registrator = new TfsStoreWithException();
            //registrator = new TFSStoreWithBug();
            var settings = new ExceptionSettings(exception.ApplicationName);
            registrator.RegisterException(exception, settings);
        }


    }
}
