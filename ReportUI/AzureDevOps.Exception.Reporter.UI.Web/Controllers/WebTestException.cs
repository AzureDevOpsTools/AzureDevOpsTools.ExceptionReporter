using System;
using System.Runtime.Serialization;

namespace AzureDevOps.Exception.Reporter.UI.Web.Controllers
{
    [Serializable]
    public class WebTestException : System.Exception
    {
        protected WebTestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public WebTestException(string message)
        :base(message)
        {
        }

        public WebTestException(string message, System.Exception inner)
        :base(message, inner)
        {
        }
    }


}