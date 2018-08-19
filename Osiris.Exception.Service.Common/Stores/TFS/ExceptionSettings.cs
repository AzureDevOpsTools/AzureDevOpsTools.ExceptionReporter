using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace Inmeta.Exception.Service.Common.Stores.TFS
{
    public static class XElementExtensions
    {
        public static string GetAttributeValue(this XElement element, string key)
        {
            var attribute = element.Attribute(key);
            return attribute != null && attribute.Value != null ? attribute.Value : string.Empty;
        }

        public static string GetElementValue(this XElement element, string key)
        {
            var subelement = element.Element(key);
            return subelement != null && subelement.Value != null ? subelement.Value : string.Empty;
        }
    }

    public class ExceptionSettings : IApplicationInfo
    {
        private readonly string applicationName;
        private readonly string settingsFile;

        public ExceptionSettings(string applicationName, string settingsFile)
        {
            this.applicationName = applicationName;
            this.settingsFile = settingsFile;

            var app = ReadApplicationIdSettings();

            if (app == null)
            {
                app = ReadDefaultSettings();
                if (app == null)
                {
                    throw new ExceptionReporterException(
                        "XML configuration file (" + settingsFile + ") does not contain an element for " + applicationName + " nor a default element");
                }
            }

            InitializeFromXElement(app);            
        }

        private XElement ReadApplicationIdSettings()
        {            
            return (from a in XElement.Load(settingsFile).Elements("Application")
             where a.GetAttributeValue("Name") == applicationName
             select a).FirstOrDefault();
        }

        private XElement ReadDefaultSettings()
        {
            return (from a in XElement.Load(settingsFile).Elements("Application")
                    where a.GetAttributeValue("Name") == "Default"
             select a).FirstOrDefault();
        }

        private void InitializeFromXElement(XElement app)
        {
            ApplicationName = app.GetAttributeValue("Name");
            TfsServer = app.GetElementValue("TFSServer");
            Collection = app.GetElementValue("Collection");
            TeamProject = app.GetElementValue("TeamProject");
            Area = app.GetElementValue("Area");
            AssignedTo = app.GetElementValue("AssignedTo");

            if (string.IsNullOrEmpty(TeamProject))
            {
                throw new ExceptionReporterException("No TeamProject in configuration file for ApplicationName: " + ApplicationName);
            }
        }

        public string ApplicationName { get; private set; }
        public string TfsServer { get; private set; }
        public string Collection { get; private set; }
        public string TeamProject{ get; private set; }
        public string Area { get; private set; }
        public string AssignedTo { get; private set; }
    }
}