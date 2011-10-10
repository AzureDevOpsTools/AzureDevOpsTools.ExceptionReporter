using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Inmeta.Exception.Service.Common
{
    [Serializable]
    [DataContract (Namespace = "http://tempuri.org/")]
    public class ExceptionEntity
    {
        [DataMember]
        public string ApplicationName { get; set; }
        
        [DataMember]
        public string Reporter { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public string ExceptionType { get; set; }

        [DataMember]
        public string ExceptionTitle { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string TheClass { get; set; }

        [DataMember]
        public string TheMethod { get; set; }

        [DataMember]
        public string TheSource { get; set; }

        [DataMember]
        public string ChangeSet { get; set; }

        [DataMember]
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
            TheClass = string.Empty;
            TheMethod = string.Empty;
            TheSource = string.Empty;
            ChangeSet = string.Empty;
            Username = string.Empty;
        }

        public ExceptionEntity(string applicationName, string reporter, string comment, string version,
                             string exceptionMessage, string exceptionType, string exceptionTitle, string stackTrace,
                             string theClass, string theMethod, string theSource, string changeSet, string username)
        {
            Contract.Requires(applicationName != null);
            Contract.Requires(reporter != null);
            Contract.Requires(comment != null);
            Contract.Requires(version != null);
            Contract.Requires(exceptionMessage != null);
            Contract.Requires(exceptionType != null);
            Contract.Requires(exceptionTitle != null);
            Contract.Requires(stackTrace != null);
            Contract.Requires(theClass != null);
            Contract.Requires(theMethod != null);
            Contract.Requires(theSource != null);
            Contract.Requires(changeSet != null);
            Contract.Requires(username != null);
            Contract.Ensures(StackTrace != null);

            ApplicationName = applicationName;
            Reporter = reporter;
            Comment = comment;
            Version = version;
            ExceptionMessage = exceptionMessage;
            ExceptionType = exceptionType;
            ExceptionTitle = exceptionTitle;
            StackTrace = stackTrace;
            TheClass = theClass;
            TheMethod = theMethod;
            TheSource = theSource;
            ChangeSet = changeSet;
            Username = username;
        }

        [ContractInvariantMethod]
        // ReSharper disable UnusedMember.Local
        private void InvariantCheck()
        // ReSharper restore UnusedMember.Local
        {
            Contract.Invariant(ApplicationName != null);
            Contract.Invariant(Reporter != null);
            Contract.Invariant(Comment != null);
            Contract.Invariant(Version != null);
            Contract.Invariant(ExceptionMessage != null);
            Contract.Invariant(ExceptionType != null);
            Contract.Invariant(ExceptionTitle != null);
            Contract.Invariant(StackTrace != null);
            Contract.Invariant(TheClass != null);
            Contract.Invariant(TheMethod != null);
            Contract.Invariant(TheSource != null);
            Contract.Invariant(ChangeSet != null);
            Contract.Invariant(Username != null);
        }
    }
}
