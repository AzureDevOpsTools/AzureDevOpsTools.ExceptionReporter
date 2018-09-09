using System;
using System.Runtime.Serialization;

namespace AzureDevOps.Exception.Service.Common
{
    [Serializable]
    public class ExceptionReporterException : System.Exception
    {
        public ExceptionReporterException()
        {
        }

        public ExceptionReporterException(string message)
            : base(message)
        {
        }

        public ExceptionReporterException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected ExceptionReporterException(SerializationInfo info,
         StreamingContext context)
            : base(info, context)
        {
        }
    }
}