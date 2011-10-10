using System;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Win32;

namespace Inmeta.Exception.Reporter
{
    internal class ReportLogger
    {
        private const string DeliveredExceptions = "DeliveredExceptions";
        private const string ExceptionReporterInfo = "ExceptionReporterInfo";
        private const string ExceptionsDuringDelivery = "ExceptionsDuringDelivery";
        private const string UnDeliveredExceptions = "UnDeliveredExceptions";

        private const string PathExtension = @"Inmeta\ExceptionReporter\";
        private const string MyRepoName = "MyRepo";
        private static ReportLogger _instance;
        private Hierarchy _repo;

        private ILog DeliveredExceptionsLogger { get; set; }
        private ILog UnDeliveredExceptionsLogger { get; set; }
        private ILog ExceptionsDuringDeliveryLogger { get; set; }
        private ILog ExceptionReporterInfoLogger { get; set; }

        private string Path
        {
            get
            {
                RegistryKey localMachine = Registry.LocalMachine;
                const string keypath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";

                //default location to 
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                //override with Registry settings if available.
                RegistryKey key = localMachine.OpenSubKey(keypath);

                if (key != null && key.GetValue("Common AppData") != null)
                    path = key.GetValue("Common AppData").ToString();

                //append Inmeta\ExceptionReporter to seperate from other logs.
                path = System.IO.Path.Combine(path, PathExtension);
                return path;
            }
        }

        internal ReportLogger()
        {
            //reset logger for every new instance.
            if (_instance != null)
                _instance._repo.Shutdown();
            _instance = null;
        }

        public static ReportLogger Instance
        {
            get { return (_instance ?? new ReportLogger()).Init(); }
        }

        private void ConfigureLogger(Logger logger)
        {
            logger.Level = Level.All;
            var patternLayout = new PatternLayout
                                    {ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"};
            patternLayout.ActivateOptions();
            var roller = new RollingFileAppender
                             {
                                 Layout = patternLayout,
                                 AppendToFile = true,
                                 RollingStyle = RollingFileAppender.RollingMode.Size,
                                 MaxSizeRollBackups = 4,
                                 MaximumFileSize = "500KB",
                                 File = System.IO.Path.Combine(Path, logger.Name + ".log")
                             };

            roller.ActivateOptions();
            logger.RemoveAllAppenders();
            logger.AddAppender(roller);
        }

        private ReportLogger Init()
        {
            try
            {
                //    log4net.Util.LogLog.InternalDebugging = true;
                //create our own logging repository, to avoid conflict with other log4net configurations.
                try
                {
                    LogManager.ShutdownRepository(MyRepoName);
                    _repo = LogManager.GetRepository(MyRepoName) as Hierarchy;
                }
                catch(LogException)
                {
                    //ignore first time
                    _repo = LogManager.CreateRepository(MyRepoName, typeof(Hierarchy)) as Hierarchy;
                }


                _repo.Name = MyRepoName;
                _repo.Root.RemoveAllAppenders();
                _repo.Root.Level = Level.All;

                ConfigureLogger(LogManager.GetLogger(_repo.Name, DeliveredExceptions).Logger as Logger);
                ConfigureLogger(LogManager.GetLogger(_repo.Name, UnDeliveredExceptions).Logger as Logger);
                ConfigureLogger(LogManager.GetLogger(_repo.Name, ExceptionsDuringDelivery).Logger as Logger);
                ConfigureLogger(LogManager.GetLogger(_repo.Name, ExceptionReporterInfo).Logger as Logger);

                DeliveredExceptionsLogger = LogManager.GetLogger(_repo.Name, DeliveredExceptions);
                UnDeliveredExceptionsLogger = LogManager.GetLogger(_repo.Name, UnDeliveredExceptions);
                ExceptionsDuringDeliveryLogger = LogManager.GetLogger(_repo.Name, ExceptionsDuringDelivery);
                ExceptionReporterInfoLogger = LogManager.GetLogger(_repo.Name, ExceptionReporterInfo);

                BasicConfigurator.Configure(_repo);
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
                ExceptionsDuringDeliveryLogger.Error("Failed to deliver TFS Exception", ex);
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
        public void LogDeliveredExceptions(TFSExceptionReport ex)
        {
            try
            {
                DeliveredExceptionsLogger.Info(ex.ExceptionEntity.GetType().
                                                   GetProperties().Select(
                                                       prop =>
                                                       string.Format("{0} = {1}",
                                                                     prop.Name,
                                                                     (prop.GetValue(ex.ExceptionEntity,
                                                                                    BindingFlags.Default, null, null,
                                                                                    null) ?? "NO VALUE").ToString()))
                                                   .Aggregate((x, y) => x + Environment.NewLine + y), null);
            }
            catch
            {
                //No more falback solutions. 
                //need to catch to avoid circular exception
            }
        }

        /// <summary>
        /// This method will log an TFSExceptionReport to local file.
        /// </summary>
        /// <param name="ex">The TFSExceptionReport beeing saved to file.</param>
        public void LogUnDeliveredExceptions(TFSExceptionReport ex)
        {
            try
            {
                //Get exception logger
                UnDeliveredExceptionsLogger.Warn(ex.ExceptionEntity.GetType().
                                                     GetProperties().Select(
                                                         prop =>
                                                         string.Format("{0} = {1}",
                                                                       prop.Name,
                                                                       (prop.GetValue(ex.ExceptionEntity,
                                                                                      BindingFlags.Default, null, null,
                                                                                      null) ?? "NO VALUE").ToString()))
                                                     .Aggregate((x, y) => x + Environment.NewLine + y));
            }
            catch
            {
                //No more falback solutions. 
                //need to catch to avoid circular exception
            }
        }

        /// <summary>
        /// Log to info file
        /// </summary>
        /// <param name="p"></param>
        public void LogExceptionReporterInfo(string p)
        {
            try
            {
                //Get exception logger
                ExceptionReporterInfoLogger.Info(p, null);
            }
            catch
            {
                //No more falback solutions. 
                //need to catch to avoid circular exception
            }
        }
    }
}