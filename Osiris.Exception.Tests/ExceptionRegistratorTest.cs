﻿using System.Configuration;
using System.IO;
using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Stores.TFS;
using System.Diagnostics.Contracts;
using System;
using Inmeta.Exception.Reporter;
using System.Linq;
using Fasterflect;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using Osiris.Exception.Tests;

namespace Inmeta.Exception.Tests
{

    
    public class ExceptionRegistratorTest : ExceptionReportingTestBase
    {
        internal readonly string SettingsFileUri = System.IO.Path.GetTempFileName();

        [SetUp]
        public void MyTestInitialize()
        {
            Contract.Requires(!String.IsNullOrEmpty(SettingsFileUri));
            var xmlContent = ExceptionTestConstants.APPLICATION_CONFIG;
            var finfo = new FileInfo(SettingsFileUri);
            var writer = finfo.CreateText();
            writer.Write(xmlContent);
            writer.Close();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            Contract.Requires(!String.IsNullOrEmpty(SettingsFileUri));
            new FileInfo(SettingsFileUri).Delete();
        }

        [Category("Integration")]
        [Test]
        public void TFSExceptionRegistrator_RegisterExceptionTest()
        {
            var settings = new ExceptionSettings(ExceptionEntityTestData.MyExceptionEntity.ApplicationName, SettingsFileUri);
            var registrator = new TFSStore();
            registrator.RegisterException(ExceptionEntityTestData.MyExceptionEntity, settings);
        }

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
            var registrator = new TFSStore();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntity_WITH_NEWLINE;

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
            var registrator = new TFSStore();
            var exceptionEntity = ExceptionEntityTestData.MyExceptionEntityMessageGT255;

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
            var registrator = new TFSStore();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntity_WITH_TABULATOR;

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
            var registrator = new TFSStore();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntity_WITH_CARRIAGE_RETURN;

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
            var registrator = new TFSStore();
            var exceptionEntity = ExceptionEntityTestData.ExceptionEntity_WITHLINESHIFT;

            registrator.RegisterException(exceptionEntity, settings);

            var ent = registrator.GetWorkItem(exceptionEntity, settings);

            //ensure values are the same.
            ent.GetType().GetProperties().ToList().ForEach(
               (prop) =>
               Assert.IsTrue(ent.GetPropertyValue(prop.Name).ToString() == exceptionEntity.GetPropertyValue(prop.Name).ToString()
               || ent.Comment.Contains(exceptionEntity.Comment))
               );
        }

        [Test]
        public void ReportLogger_EnsureLogFilesExists()
        {
            var reportLogger = typeof(ExceptionRegistrator).Assembly.
                GetType("Inmeta.Exception.Reporter.ReportLogger", true).CreateInstance();

            //delete all files in log dir
            var programdataInmetaExceptionreport = Environment.ExpandEnvironmentVariables(reportLogger.GetPropertyValue("Path").ToString());

            if (Directory.Exists(programdataInmetaExceptionreport))
                Directory.GetFiles(programdataInmetaExceptionreport)
                .ToList().ForEach((f) => new FileInfo(f).Delete());

            //init logger.
            reportLogger.CallMethod("Init");
            
            reportLogger.CallMethod("LogDeliveredExceptions", 
                new TFSExceptionReport("TEST", "TEST", "TEST", 
                    new System.Exception("FOO", new System.Exception("INNER FOO"))));

            reportLogger.CallMethod("LogExceptionsDuringDelivery", new System.Exception("FOO", new System.Exception("INNER FOO")));
            reportLogger.CallMethod("LogExceptionReporterInfo", "YAFM, Yet Anther FOO Message");
            reportLogger.CallMethod("LogUnDeliveredExceptions", new TFSExceptionReport("TEST", "TEST", "TEST", new System.Exception("FOO", new System.Exception("INNER FOO"))));
        
            var names =string.Empty;
            ((Hierarchy) reportLogger.GetFieldValue("_repo")).GetCurrentLoggers().ToList().ForEach((logger) => names += " " + logger.Name + ".log");

            //ensure files exists.
            //loop over all files in log dir and make sure no other files than log files exists 
            //and that they are all there!
            Directory.GetFiles(programdataInmetaExceptionreport)
                .ToList().ForEach((f) =>
                                      {
                                          //check this file is part of log files list
                                          var fileNameWithoutExtension = Path.GetFileName(f);

                                          Assert.IsTrue(names.Contains(fileNameWithoutExtension), f + " - is not a Log file:");
                                          //remove file from list of logs
                                          names = names.Replace(" " + fileNameWithoutExtension, String.Empty);
                                      });
        
            Assert.IsTrue(names.Trim().Length == 0, "The following log files where not created " + names);
        }
    }

    public class ExceptionEntityTestData
    {
        public static ExceptionEntity MyExceptionEntity
        {
            get
            {
                return new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
                    "anonym",
                    "no comments",
                    "1.0",
                    "My exception message",
                    "My.Type",
                    "This is the exception",
                    "trace;trace;trace;trace;3",
                    "Osiris.Exception.Tests.ExceptionRegistratorTest",
                    "SomeMethod",
                    "this is the source",
                    string.Empty,
                    "the user"
                    );
            }
        }

        public static ExceptionEntity MyExceptionEntityMessageGT255
        {
            get
            {
                return new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
                    "anonym",
                    "no comments",
                    "1.0",
                    ExceptionTestConstants.RndStrLength(260),
                    "My.Type",
                    "This is the exception",
                    //ensure new item all the tiem
                    "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(30),
                    "Osiris.Exception.Tests.ExceptionRegistratorTest",
                    "SomeMethod",
                    "this is the source",
                    String.Empty,
                    "the user"
                    );
            }
        }

        public static ExceptionEntity ExceptionEntity_WITH_NEWLINE
        {

            get
            {
                return new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
                    "anonym",
                    "no comments",
                    "1.0",
                    ExceptionTestConstants.RndStrLength(30) + "\n" + ExceptionTestConstants.RndStrLength(30),
                    "My.Type",
                    "This is the exception",
                    "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
                    "Osiris.Exception.Tests.ExceptionRegistratorTest",
                    "SomeMethod",
                    "this is the source",
                    String.Empty,
                    "the user"
                    );
            }
        }

        public static ExceptionEntity ExceptionEntity_WITH_CARRIAGE_RETURN
        {

            get
            {
                return new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
                    "anonym",
                    "no comments",
                    "1.0",
                    ExceptionTestConstants.RndStrLength(30) + "\r" + ExceptionTestConstants.RndStrLength(30),
                    "My.Type",
                    "This is the exception",
                    "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
                    "Osiris.Exception.Tests.ExceptionRegistratorTest",
                    "SomeMethod",
                    "this is the source",
                    String.Empty,
                    "the user"
                    );
            }
        }

        public static ExceptionEntity ExceptionEntity_WITH_TABULATOR
        {

            get
            {
                return new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
                    "anonym",
                    "no comments",
                    "1.0",
                    ExceptionTestConstants.RndStrLength(30) + "\t" + ExceptionTestConstants.RndStrLength(30),
                    "My.Type",
                    "This is the exception",
                    "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
                    "Osiris.Exception.Tests.ExceptionRegistratorTest",
                    "SomeMethod",
                    "this is the source",
                    String.Empty,
                    "the user"
                    );
            }
        }

        public static ExceptionEntity ExceptionEntity_WITHLINESHIFT
        {

            get {
                return new ExceptionEntity(ExceptionTestConstants.APPLICATION_NAME,
                    "anonym",
                    "no comments",
                    "1.0",
                    ExceptionTestConstants.RndStrLength(30) + System.Environment.NewLine + ExceptionTestConstants.RndStrLength(30),
                    "My.Type",
                    "This is the exception",
                    "trace;trace;trace;trace;3" + ExceptionTestConstants.RndStrLength(300),
                    "Osiris.Exception.Tests.ExceptionRegistratorTest",
                    "SomeMethod",
                    "this is the source",
                    String.Empty,
                    "the user"
                    );
            }
        }
    }
}
