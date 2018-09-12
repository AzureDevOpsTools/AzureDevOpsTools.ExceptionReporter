using System;

namespace AzureDevOps.Exception.Service.Common
{
    public class DefaultServiceSettings : ServiceSettings
    {
        public DefaultServiceSettings()
            : base(new Uri("http://exceptions.osiris.no/Service.asmx"), "OsirisExceptionReporter", "1qaz2WSX")
        {}
    }
}