namespace AzureDevOps.Exception.Reporter
{
    public class ExceptionHandlerSettings
    {
        public ExceptionHandlerSettings(string applicationName, bool tryContinueAfterException, bool showExitAppWindow,
            string serviceUrl = "http://exceptions.km.kongsberg.com/web/service.asmx", bool logExceptionReports = true, bool useReportingUi = true)
        {
            ApplicationName = applicationName;
            TryContinueAfterException = tryContinueAfterException;
            ShowExitWindow = showExitAppWindow;
            ServiceUrl = serviceUrl;
            LogExceptionReports = logExceptionReports;
            UseReportingUi = useReportingUi;
        }

        public string ApplicationName { get; private set; }
        public bool TryContinueAfterException { get; private set; }
        public bool ShowExitWindow { get; private set; }
        public bool LogExceptionReports { get; set; }
        public bool UseReportingUi { get; set; }
        public string ServiceUrl { get; set; }
    }
}
