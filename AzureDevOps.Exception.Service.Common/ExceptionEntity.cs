using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace AzureDevOps.Exception.Service.Common
{
    [Serializable]
    [DataContract(Namespace = "http://exceptions.maritimesim.com/")]
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
        }

        public string GetSerialized()
        {
            var serializer = new DataContractSerializer(typeof(ExceptionEntity));
            var output = new StringWriter();
            using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
            {
                serializer.WriteObject(writer, this);
            }
            return output.GetStringBuilder().ToString();
        }
    }
}
