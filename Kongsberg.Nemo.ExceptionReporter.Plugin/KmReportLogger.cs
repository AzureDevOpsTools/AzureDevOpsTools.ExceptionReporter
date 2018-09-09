using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AzureDevOps.Exception.Reporter.TFSExeptionService;
using log4net;
using log4net.Repository.Hierarchy;
using System.Xml;
using log4net.Config;
using System.Reflection;

namespace AzureDevOps.ExceptionReporter.Km
{
    internal class KmReportLogger
    {
        private static readonly string XMLReporterLogger = "XMLExceptionReporter";
        private static readonly string ReporterLogger = "ExceptionReporter";
        private static readonly string ReportFailedLogger = "ExceptionReporterFailed";
        private static readonly string MyRepoName = "MyRepo";

        private static KmReportLogger _instance;
        private Hierarchy _repo;

        internal bool LogExceptionReports { get; set; }

        private static string XMLConfig = @"  <log4net>" + Environment.NewLine +
                                    @"    <appender name=""XMLReporterRollingFileAppender"" " +
                                    @"type=""log4net.Appender.RollingFileAppender"">" + Environment.NewLine +
                                    @"      <file type=""log4net.Util.PatternString"">" + Environment.NewLine +
                                    @"        <converter>" + Environment.NewLine +
                                    @"          <name value=""folder""/>" + Environment.NewLine +
                                    @"          <type value=""Kongsberg.Nemo.ExceptionReporter.FilePatternConverter,Kongsberg.Nemo.ExceptionReporter"" />" + Environment.NewLine +
                                    @"        </converter>" + Environment.NewLine +
                                    @"        <conversionPattern value=""%folder\XMLExceptionReporter.log"" />" + Environment.NewLine +
                                    @"      </file>" + Environment.NewLine +
                                    @"      <appendToFile value=""true"" />" + Environment.NewLine +
                                    @"      <rollingStyle value=""Size"" />" + Environment.NewLine +
                                    @"      <maximumFileSize value=""1000KB"" />" + Environment.NewLine +
                                    @"      <maxSizeRollBackups value=""5"" />" + Environment.NewLine +
                                    @"      <layout type=""log4net.Layout.PatternLayout"">" + Environment.NewLine +
                                    @"        <conversionPattern value=""%message%newline"" />        " + Environment.NewLine +
                                    @"      </layout>" + Environment.NewLine +
                                    @"    </appender>" + Environment.NewLine +
                                    @"    <appender name=""ExceptionReporterRollingFileAppender"" " +
                                    @"type=""log4net.Appender.RollingFileAppender"">" + Environment.NewLine +
                                    @"      <file type=""log4net.Util.PatternString"">" + Environment.NewLine +
                                    @"        <converter>" + Environment.NewLine +
                                    @"          <name value=""folder""/>" + Environment.NewLine +
                                    @"          <type value=""Kongsberg.Nemo.ExceptionReporter.FilePatternConverter,Kongsberg.Nemo.ExceptionReporter"" />" + Environment.NewLine +
                                    @"        </converter>" + Environment.NewLine +
                                    @"        <conversionPattern value=""%folder\ExceptionReporter.log"" />" + Environment.NewLine +
                                    @"      </file>" + Environment.NewLine +
                                    @"      <appendToFile value=""true"" />" + Environment.NewLine +
                                    @"      <rollingStyle value=""Size"" />" + Environment.NewLine +
                                    @"      <maximumFileSize value=""500KB"" />" + Environment.NewLine +
                                    @"      <maxSizeRollBackups value=""4"" />" + Environment.NewLine +
                                    @"      <layout type=""log4net.Layout.PatternLayout"">" + Environment.NewLine +
                                    @"        <conversionPattern value=""%date [%thread] %-5level %logger - %message%newline"" />        " + Environment.NewLine +
                                    @"      </layout>" + Environment.NewLine +
                                    @"    </appender>" + Environment.NewLine +
                                    @"    <appender name=""ExceptionReporterFailedReportRollingFileAppender"" " +
                                    @"type=""log4net.Appender.RollingFileAppender"">" + Environment.NewLine +
                                    @"      <file type=""log4net.Util.PatternString"">" + Environment.NewLine +
                                    @"        <converter>" + Environment.NewLine +
                                    @"          <name value=""folder""/>" + Environment.NewLine +
                                    @"          <type value=""Kongsberg.Nemo.ExceptionReporter.FilePatternConverter,Kongsberg.Nemo.ExceptionReporter"" />" + Environment.NewLine +
                                    @"        </converter>" + Environment.NewLine +
                                    @"        <conversionPattern value=""%folder\ExceptionReporterFailedDelivery.log"" />" + Environment.NewLine +
                                    @"      </file>" + Environment.NewLine +
                                    @"      <appendToFile value=""true"" />" + Environment.NewLine +
                                    @"      <rollingStyle value=""Size"" />" + Environment.NewLine +
                                    @"      <maximumFileSize value=""500KB"" />" + Environment.NewLine +
                                    @"      <maxSizeRollBackups value=""4"" />" + Environment.NewLine +
                                    @"      <layout type=""log4net.Layout.PatternLayout"">" + Environment.NewLine +
                                    @"        <conversionPattern value=""%date [%thread] %-5level %logger - %message%newline"" />" + Environment.NewLine +
                                    @"      </layout>" + Environment.NewLine +
                                    @"    </appender>" + Environment.NewLine +
                                    Environment.NewLine +
                                    @"    <logger name=""ExceptionReporter"">" + Environment.NewLine +
                                    @"      <appender-ref ref=""ExceptionReporterRollingFileAppender"" />" + Environment.NewLine +
                                    @"      <level value=""INFO"" />" + Environment.NewLine +
                                    @"    </logger>" + Environment.NewLine +
                                    Environment.NewLine +
                                    @"    <logger name=""ExceptionReporterFailed"">" + Environment.NewLine +
                                    @"      <appender-ref ref=""ExceptionReporterFailedReportRollingFileAppender"" />" + Environment.NewLine +
                                    @"      <level value=""INFO"" />" + Environment.NewLine +
                                    @"    </logger>" + Environment.NewLine +
                                    @"    " + Environment.NewLine +
                                    @"    <logger name=""XMLExceptionReporter"">" + Environment.NewLine +
                                    @"      <appender-ref ref=""XMLReporterRollingFileAppender"" />" + Environment.NewLine +
                                    @"      <level value=""INFO"" />" + Environment.NewLine +
                                    @"    </logger>" + Environment.NewLine +
                                    @"    " + Environment.NewLine +
                                    @"  </log4net>";

        internal KmReportLogger()
        {
            //reset logger for every new instance.
            if (_instance != null)
                _instance._repo.Shutdown();
            _instance = null;
            Init();
        }

        public static KmReportLogger Instance
        {
            get { return _instance ?? new KmReportLogger(); }
        }

        private KmReportLogger Init()
        {
            try
            {
                //create our own logging repository, to avoid conflict with other log4net configurations.
                _repo = LogManager.CreateRepository(MyRepoName, typeof(Hierarchy)) as Hierarchy;
                _repo.Name = MyRepoName;

                var doc = new XmlDocument();
                doc.LoadXml(XMLConfig);
                XmlConfigurator.Configure(_repo, doc.DocumentElement);
            }
            catch
            {
                //failed to init Log4Net logger...
            }

            _instance = this;
            return _instance;
        }


        /// <summary>
        /// use this method to log to file when an exception report failed to deliver. 
        /// </summary>
        /// <param name="ex">The exception occured during delivery failure.</param>
        public void LogExceptionsDuringDelivery(System.Exception ex)
        {
            try
            {
                var _log = LogManager.GetLogger(_repo.Name, ReportFailedLogger);
                if (_log != null)
                    _log.Error("Failed to deliver TFS Exception", ex);
            }
            catch
            {
                //No more fallback solutions
                //need to catch to avoid circular exception
            }
        }

        

        /// <summary>
        /// This method will log an TFSExceptionReport to local file.
        /// </summary>
        /// <param name="ex">The TFSExceptionReport beeing saved to file.</param>
        public void LogToFile(KmTFSExceptionReport ex)
        {
            try
            {
                //Get exception logger
                var _log = LogManager.GetLogger(_repo.Name, ReporterLogger);


                //get exceptionentity
                // ReSharper disable PossibleNullReferenceException
                object exEnt = ex.ExceptionEntity;
                // ReSharper restore PossibleNullReferenceException

                if (_log != null && LogExceptionReports)
                    _log.Info(exEnt.GetType().
                              GetProperties().Select(
                                  (prop) =>
                                  prop.Name + " = " +
                                  (prop.GetValue(exEnt, BindingFlags.Default, null, null, null) ?? "NO VALUE").ToString())
                              .Aggregate((x, y) => x + System.Environment.NewLine + y));
            }
            catch
            {
                //No more falback solutions. 
                //need to catch to avoid circular exception
            }

            LogToFileAsXML(ex);
        }

        public void LogToFileAsXML(KmTFSExceptionReport ex)
        {
            try
            {
                //Ge{t exception logger
                var _log = LogManager.GetLogger(_repo.Name, XMLReporterLogger);

                var builder = new StringBuilder();
                using (var strWrt = new StringWriter(builder))
                {
                    var ser = new XmlSerializer(typeof(ExceptionEntity));
                    var outStr = new StringBuilder();
                    using (var mem = new StringWriter(outStr))
                    {
                        using (var writer = new XmlTextWriter(mem))
                        {
                            writer.Formatting = Formatting.Indented;
                            ser.Serialize(writer, ex.ExceptionEntity);
                        }
                    }
                    _log.Info(outStr.ToString());
                }
            }
            catch
            {
                //No more falback solutions. 
                //need to catch to avoid circular exception
            }
        }

        internal void LogInfo(string p)
        {
            try
            {
                //Get exception logger
                LogManager.GetLogger(_repo.Name, ReportFailedLogger).Info(p);
            }
            catch
            {
                //No more falback solutions. 
                //need to catch to avoid circular exception
            }

        }
    }
}
