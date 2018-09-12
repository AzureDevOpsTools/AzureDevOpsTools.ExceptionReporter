using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace AzureDevOps.Exception.Service.Common
{
    public class ExceptionEntity
    {
        public string ApplicationName { get; set; }

        public string Reporter { get; set; }

        public string Comment { get; set; }

        public string Version { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionTitle { get; set; }

        public string StackTrace { get; set; }

        public string ExceptionClass { get; set; }

        public string ExceptionMethod { get; set; }

        public string ExceptionSource { get; set; }

        public string ChangeSet { get; set; }

        public string Username { get; set; }

        public ExceptionEntity()
        {
            ApplicationName = string.Empty;
            Reporter = string.Empty;
            Comment = string.Empty;
            Version = string.Empty;
            ExceptionMessage = string.Empty;
            ExceptionType = string.Empty;
            ExceptionTitle = string.Empty;
            StackTrace = string.Empty;
            ExceptionClass = string.Empty;
            ExceptionMethod = string.Empty;
            ExceptionSource = string.Empty;
            ChangeSet = string.Empty;
            Username = string.Empty;
        }

        public ExceptionEntity(string applicationName, string reporter, string comment, string version,
                             string exceptionMessage, string exceptionType, string exceptionTitle, string stackTrace,
                             string theClass, string theMethod, string theSource, string changeSet, string username)
        {
            ApplicationName = applicationName;
            Reporter = reporter;
            Comment = comment;
            Version = version;
            ExceptionMessage = exceptionMessage;
            ExceptionType = exceptionType;
            ExceptionTitle = exceptionTitle;
            StackTrace = stackTrace;
            ExceptionClass = theClass;
            ExceptionMethod = theMethod;
            ExceptionSource = theSource;
            ChangeSet = changeSet;
            Username = username;
        }
    }
}
