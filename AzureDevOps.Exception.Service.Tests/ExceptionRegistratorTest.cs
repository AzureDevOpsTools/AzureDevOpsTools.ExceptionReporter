using System.IO;
using AzureDevOps.Exception.Service.Common;
using AzureDevOps.Exception.Service.Common.Stores.TFS;
using System;
using System.Linq;
using Fasterflect;
using NUnit.Framework;

namespace AzureDevOps.Exception.Tests
{
    public class ExceptionRegistratorTest : ExceptionReportingTestBase
    {
        internal readonly string SettingsFileUri = Path.GetTempFileName();

        [SetUp]
        public void MyTestInitialize()
        {
            var xmlContent = ExceptionTestConstants.APPLICATION_CONFIG;
            var finfo = new FileInfo(SettingsFileUri);
            var writer = finfo.CreateText();
            writer.Write(xmlContent);
            writer.Close();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            new FileInfo(SettingsFileUri).Delete();
        }

        //[Category("Integration")]
        //[Test]
        //public void TFSExceptionRegistrator_RegisterExceptionTest()
        //{
        //    var settings = new ExceptionSettings(ExceptionEntityTestData.MyExceptionEntity.ApplicationName, SettingsFileUri);
        //    var registrator = new TFSStoreWithBug();
        //    registrator.RegisterException(ExceptionEntityTestData.MyExceptionEntity, settings);
        //}

        /// <summary>
        /// This test actually creates an exception-workitem on the TFS test-server. 
        /// If the exception-workitem type is ever updated, the new version should be uploaded 
        /// to the TFSExceptionReporterTest-project on VM-TFS-TEST, to ensure the exception service doesn't choke on it.
        /// </summary>
        [Test]
        [Category("Integration")]
        public void TFSExceptionRegistrator_Creation_with_new_line()
        {
            var settings = new ExceptionSettings(ExceptionEntityTestData.MyExceptionEntity.ApplicationName, SettingsFileUri);
            var registrator = new TfsStoreWithException();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntityWithNewline;

            registrator.RegisterException(exceptionEntity, settings);

            var ent = registrator.GetWorkItem(exceptionEntity, settings);

            //ensure values are the same.
            ent.GetType().GetProperties().ToList().ForEach(
               (prop) =>
               Assert.IsTrue(ent.GetPropertyValue(prop.Name).ToString() == exceptionEntity.GetPropertyValue(prop.Name).ToString()
               || ent.Comment.Contains(exceptionEntity.Comment))
               );
        }


        /// <summary>
        /// This test actually creates an exception-workitem on the TFS test-server. 
        /// If the exception-workitem type is ever updated, the new version should be uploaded 
        /// to the TFSExceptionReporterTest-project on VM-TFS-TEST, to ensure the exception service doesn't choke on it.
        /// </summary>
        [Test]
        [Category("Integration")]
        public void TFSExceptionRegistrator_Creation_with_message_GT_255()
        {
            var settings = new ExceptionSettings(ExceptionEntityTestData.MyExceptionEntity.ApplicationName, SettingsFileUri);
            var registrator = new TfsStoreWithException();
            var exceptionEntity = ExceptionEntityTestData.MyExceptionEntityMessageGt255;

            registrator.RegisterException(exceptionEntity, settings);

            var ent = registrator.GetWorkItem(exceptionEntity, settings);

            //ensure values are the same.
            ent.GetType().GetProperties().ToList().ForEach(
               (prop) =>
               Assert.IsTrue(ent.GetPropertyValue(prop.Name).ToString() == exceptionEntity.GetPropertyValue(prop.Name).ToString()
               || ent.Comment.Contains(exceptionEntity.Comment))
               );
        }

        /// <summary>
        /// This test actually creates an exception-workitem on the TFS test-server. 
        /// If the exception-workitem type is ever updated, the new version should be uploaded 
        /// to the TFSExceptionReporterTest-project on VM-TFS-TEST, to ensure the exception service doesn't choke on it.
        /// </summary>
        [Test]
        [Category("Integration")]
        public void TFSExceptionRegistrator_Creation_with_Tab()
        {
            var settings = new ExceptionSettings(ExceptionEntityTestData.MyExceptionEntity.ApplicationName, SettingsFileUri);
            var registrator = new TfsStoreWithException();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntityWithTabulator;

            registrator.RegisterException(exceptionEntity, settings);

            var ent = registrator.GetWorkItem(exceptionEntity, settings);

            //ensure values are the same.
            ent.GetType().GetProperties().ToList().ForEach(
               (prop) =>
               Assert.IsTrue(ent.GetPropertyValue(prop.Name).ToString() == exceptionEntity.GetPropertyValue(prop.Name).ToString()
               || ent.Comment.Contains(exceptionEntity.Comment))
               );
        }


        /// <summary>
        /// This test actually creates an exception-workitem on the TFS test-server. 
        /// If the exception-workitem type is ever updated, the new version should be uploaded 
        /// to the TFSExceptionReporterTest-project on VM-TFS-TEST, to ensure the exception service doesn't choke on it.
        /// </summary>
        [Test]
        [Category("Integration")]
        public void TFSExceptionRegistrator_Creation_with_Carriage_return()
        {
            var settings = new ExceptionSettings(ExceptionEntityTestData.MyExceptionEntity.ApplicationName, SettingsFileUri);
            var registrator = new TfsStoreWithException();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntityWithCarriageReturn;

            registrator.RegisterException(exceptionEntity, settings);

            var ent = registrator.GetWorkItem(exceptionEntity, settings);

            //ensure values are the same.
            ent.GetType().GetProperties().ToList().ForEach(
               (prop) =>
               Assert.IsTrue(ent.GetPropertyValue(prop.Name).ToString() == exceptionEntity.GetPropertyValue(prop.Name).ToString()
               || ent.Comment.Contains(exceptionEntity.Comment))
               );
        }

        /// <summary>
        /// This test actually creates an exception-workitem on the TFS test-server. 
        /// If the exception-workitem type is ever updated, the new version should be uploaded 
        /// to the TFSExceptionReporterTest-project on VM-TFS-TEST, to ensure the exception service doesn't choke on it.
        /// </summary>
        [Test]
        [Category("Integration")]
        public void TFSExceptionRegistrator_with_message_With_LineShift()
        {
            var settings = new ExceptionSettings(ExceptionEntityTestData.MyExceptionEntity.ApplicationName, SettingsFileUri);
            var registrator = new TfsStoreWithException();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntityWithlineshift;

            registrator.RegisterException(exceptionEntity, settings);

            var ent = registrator.GetWorkItem(exceptionEntity, settings);

            //ensure values are the same.
            ent.GetType().GetProperties().ToList().ForEach(
               (prop) =>
               Assert.IsTrue(ent.GetPropertyValue(prop.Name).ToString() == exceptionEntity.GetPropertyValue(prop.Name).ToString()
               || ent.Comment.Contains(exceptionEntity.Comment))
               );
        }

        //[Test]
        //public void ReportLogger_EnsureLogFilesExists()
        //{
        //    var reportLogger = typeof(ExceptionRegistrator).Assembly.
        //        GetType("Inmeta.Exception.Reporter.ReportLogger", true).CreateInstance();

        //    //delete all files in log dir
        //    var programdataInmetaExceptionreport = Environment.ExpandEnvironmentVariables(reportLogger.GetPropertyValue("Path").ToString());

        //    if (Directory.Exists(programdataInmetaExceptionreport))
        //        Directory.GetFiles(programdataInmetaExceptionreport)
        //        .ToList().ForEach((f) => new FileInfo(f).Delete());

        //    //init logger.
        //    reportLogger.CallMethod("Init");
            
        //    reportLogger.CallMethod("LogDeliveredExceptions", 
        //        new TFSExceptionReport("TEST", "TEST", "TEST", 
        //            new System.Exception("FOO", new System.Exception("INNER FOO"))));

        //    reportLogger.CallMethod("LogExceptionsDuringDelivery", new System.Exception("FOO", new System.Exception("INNER FOO")));
        //    reportLogger.CallMethod("LogExceptionReporterInfo", "YAFM, Yet Anther FOO Message");
        //    reportLogger.CallMethod("LogUnDeliveredExceptions", new TFSExceptionReport("TEST", "TEST", "TEST", new System.Exception("FOO", new System.Exception("INNER FOO"))));
        
        //    //var names =string.Empty;
        //    //( reportLogger.GetFieldValue("_repo")).GetCurrentLoggers().ToList().ForEach((logger) => names += " " + logger.Name + ".log");

        //    //ensure files exists.
        //    //loop over all files in log dir and make sure no other files than log files exists 
        //    //and that they are all there!
        //    Directory.GetFiles(programdataInmetaExceptionreport)
        //        .ToList().ForEach((f) =>
        //                              {
        //                                  //check this file is part of log files list
        //                                  var fileNameWithoutExtension = Path.GetFileName(f);

        //                                  Assert.IsTrue(names.Contains(fileNameWithoutExtension), f + " - is not a Log file:");
        //                                  //remove file from list of logs
        //                                  names = names.Replace(" " + fileNameWithoutExtension, string.Empty);
        //                              });
        
        //    Assert.IsTrue(names.Trim().Length == 0, "The following log files where not created " + names);
        //}
    }

    public class ExceptionEntityTestData
    {
        public static ExceptionEntity MyExceptionEntity => new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
            "anonym",
            "no comments",
            "1.0",
            "My exception message",
            "My.Type",
            "This is the exception",
            "trace;trace;trace;trace;3",
            "AzureDevOps.Exception.Tests.ExceptionRegistratorTest",
            "SomeMethod",
            "this is the source",
            string.Empty,
            "the user"
        );

        public static ExceptionEntity MyExceptionEntityMessageGt255 => new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
            "anonym",
            "no comments",
            "1.0",
            ExceptionTestConstants.RndStrLength(260),
            "My.Type",
            "This is the exception",
            //ensure new item all the tiem
            "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(30),
            "AzureDevOps.Exception.Tests.ExceptionRegistratorTest",
            "SomeMethod",
            "this is the source",
            string.Empty,
            "the user"
        );

        public static ExceptionEntity ExceptionEntityWithNewline => new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
            "anonym",
            "no comments",
            "1.0",
            ExceptionTestConstants.RndStrLength(30) + "\n" + ExceptionTestConstants.RndStrLength(30),
            "My.Type",
            "This is the exception",
            "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
            "AzureDevOps.Exception.Tests.ExceptionRegistratorTest",
            "SomeMethod",
            "this is the source",
            string.Empty,
            "the user"
        );

        public static ExceptionEntity ExceptionEntityWithCarriageReturn => new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
            "anonym",
            "no comments",
            "1.0",
            ExceptionTestConstants.RndStrLength(30) + "\r" + ExceptionTestConstants.RndStrLength(30),
            "My.Type",
            "This is the exception",
            "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
            "AzureDevOps.Exception.Tests.ExceptionRegistratorTest",
            "SomeMethod",
            "this is the source",
            string.Empty,
            "the user"
        );

        public static ExceptionEntity ExceptionEntityWithTabulator => new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
            "anonym",
            "no comments",
            "1.0",
            ExceptionTestConstants.RndStrLength(30) + "\t" + ExceptionTestConstants.RndStrLength(30),
            "My.Type",
            "This is the exception",
            "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
            "AzureDevOps.Exception.Tests.ExceptionRegistratorTest",
            "SomeMethod",
            "this is the source",
            string.Empty,
            "the user"
        );

        public static ExceptionEntity ExceptionEntityWithlineshift => new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
            "anonym",
            "no comments",
            "1.0",
            ExceptionTestConstants.RndStrLength(30) + Environment.NewLine + ExceptionTestConstants.RndStrLength(30),
            "My.Type",
            "This is the exception",
            "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
            "AzureDevOps.Exception.Tests.ExceptionRegistratorTest",
            "SomeMethod",
            "this is the source",
            string.Empty,
            "the user"
        );
    }
}
