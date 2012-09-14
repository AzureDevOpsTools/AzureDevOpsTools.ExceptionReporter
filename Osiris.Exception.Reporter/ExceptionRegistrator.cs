using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using System.Reflection;

using Inmeta.Exception.Service.Common;

namespace Inmeta.Exception.Reporter
{
    public class ExceptionRegistrator
    {
        public ExceptionRegistrator(string applicationName, bool useReportingUI, ServiceSettings settings, string mefCatalog = null)
        {
            ReportingUI = useReportingUI;
            ApplicationName = applicationName;
            ServiceSettings = settings;
            MEFCatalog = mefCatalog ?? (String.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : AppDomain.CurrentDomain.RelativeSearchPath);

            Register();
        }

        

        public ExceptionRegistrator(ExceptionHandlerSettings properites, string mefCatalog = null)
        {
            _properites = properites;

            MEFCatalog = mefCatalog ?? (String.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : AppDomain.CurrentDomain.RelativeSearchPath);

            Register();
        }

        private ExceptionHandlerSettings _properites = null;

        private  CompositionContainer _container;
        private readonly ExceptionReportInterfaces _interfaces = new ExceptionReportInterfaces();
        private readonly ExceptionReportPluginInterface _interfacesPl = new ExceptionReportPluginInterface();
        public ServiceSettings ServiceSettings { get; private set; }

        private  readonly object _syncObject = new object();

        private  object PreviousException { get; set; }
        public  bool ReportingUI { get; set; }
        private string MEFCatalog { get;set;}
        /// <summary>
        /// The application name.
        /// In nemo project this is HARD CODED TO "Instructor", should be able to report from another application name.
        /// </summary>
        private string ApplicationName { get; set; }
        
        
        
        /// <summary>
        /// The exception beeing reported.
        /// </summary>
        private  System.Exception TheException { get; set; }

        internal  IExceptionReportView Reportform { get { return _interfaces.Reportform; } }

        internal  IExceptionTrappingStrategy TrappingStrategy { get { return _interfaces.TrappingStrategy; } }

        internal  IExceptionHandler Handler { get { return _interfacesPl.ExceptionHandler; } }

        /// <summary>
        /// Call this to register the excpetion trapper. 
        /// This function should be the first function called in your application, it MUST be called before any forms are created. 
        /// Good  practice is to call it before Application.Run().
        /// </summary>
        private void Register()
        {
            //this code does not follow best practice in terms of compile time type checking. 
            //But this library will be used at the very low end of the component stack and we want 
            //to limit the amount of configuration bubbling up to application level to the minimum. 

            //get the registered plugins.
            CreatePlugins();

            if ((Handler == null && _properites != null) || (Handler != null && _properites == null))
            {
                throw new ArgumentException(
                    "Unable to initialize exception reporter. Plugin type mismatch at initialization.");
            }
            //make the Trapper register unhandled exceptions. 
            //Since this is done differently depending on application type,
            //The delegate provided is the event to call when an excpetion occurs.
            if (Handler == null)
            {
                TrappingStrategy.RegisterExceptionEvents(OnException);
                ReportLogger.Instance.LogExceptionReporterInfo("Registered with Trapper = " + TrappingStrategy +
                                                               ", View = " + Reportform);
            }
            else
            {
                Handler.Init(_properites);
            }
        }

        private void OnException(System.Exception e, bool isTerminating)
        {
            //only report one exception at the time.
            ReportLogger.Instance.LogExceptionReporterInfo("Received exception (isTerminating = " + isTerminating + ")");

            lock (_syncObject)
            {
                //avoid recursive reporting since we have registered both Main Form and AppDomain with unhandled exceptions
                if (PreviousException != null && e != null && PreviousException.ToString() == e.ToString())
                {
                    ReportLogger.Instance.LogExceptionReporterInfo(
                        new ArgumentException("Trying to report on the same excpetion.", e).ToString());
                    //same as previous
                    return;
                }

                PreviousException = e;
                TheException = e;

                try
                {
                    //use ExceptionHandler.UseReportGUI to control if UI is to be used or not.
                    if (!ReportingUI)
                    {
                        OnPost("Exception reported w/o description");
                        return;
                    }

                    //Same as NEMO project:
                    var errorText = Common.Instance.CreateExceptionText(e);

                    //show the error to the user and collect a description of the error from the user.
                    try
                    {
                        //show the exception using the registered IReportForm.
                        //provide the text to display: Same as NEMO project, but now the form is not part of this assembly so we generate the error text it and provide it to the form.
                        Reportform.ShowException(errorText,
                                            OnPost,
                                            OnCancel);
                    }
                    catch (System.Exception ex)
                    {
                        OnCancel("THIS IS A AUTO GENERATED TEXT: Failed to show exception report.");
                        ReportLogger.Instance.LogExceptionsDuringDelivery(
                            new InvalidOperationException("Failed to show exception report.",
                                                    ex));
                    }
                }
                finally
                {
                    if (isTerminating && ReportingUI)
                        Reportform.ShowTerminateDialog();
                }
            }
            
        }

        private void OnCancel(string description)
        {
            //send cancel report exception to log
            var report =
                new TFSExceptionReport(
                    ApplicationName,
                    Common.Instance.Reporter,
                    Common.Instance.Reporter,
                    TheException,
                    Common.Instance.Version,
                    description ?? "Exception reported w/o description");

            ReportLogger.Instance.LogUnDeliveredExceptions(report);
        }

        private void OnPost(string description)
        {
            try
            {
                //create exception entity
                var report = new TFSExceptionReport
                    (
                    ApplicationName,
                    Common.Instance.Reporter,
                    Common.Instance.Reporter, 
                    TheException,
                    Common.Instance.Version,
                    description);

                //post to service. 
                System.Exception result = Post(report);

                ReportLogger.Instance.LogExceptionReporterInfo("Result posted");

                //if error show to user.
                if (result != null)
                {
                    ReportLogger.Instance.LogExceptionReporterInfo("Failed delivery.");
                    InvokeDeliveryFailure(result.Message);
                }
            }
            catch (System.Exception ex)
            {
                ReportLogger.Instance.LogExceptionsDuringDelivery(
                    new System.Exception("Exception during TFSExceptionREport create or post", ex));

                InvokeDeliveryFailure(ex.Message);
            }
        }

        public System.Exception Post(TFSExceptionReport report)
        {
            try
            {
                // Check for settings overrides
                var serviceUrl = ServiceSettings.ServiceUrl;

                //set to default 
                var serviceCredentials = CredentialCache.DefaultNetworkCredentials;

                //if credentials are not set return with exception.
                if (!String.IsNullOrEmpty(ServiceSettings.Username) && !String.IsNullOrEmpty(ServiceSettings.Password))
                    serviceCredentials = new NetworkCredential(ServiceSettings.Username, ServiceSettings.Password, ServiceSettings.Domain);

                using (var client = new TFSExeptionService.Service { Url = serviceUrl.ToString(), Credentials = serviceCredentials })
                {
                    client.AddNewApplicationException(report.ExceptionEntity);
                }
                //save this report to local disk
                ReportLogger.Instance.LogDeliveredExceptions(report);
            }
            catch (System.Exception e)
            {
                //save this report as undelivered to local disk
                ReportLogger.Instance.LogUnDeliveredExceptions(report);

                ReportLogger.Instance.LogExceptionsDuringDelivery(new System.Exception("Trying to send to service at = " + ServiceSettings.ServiceUrl, e));

                //change in behavior: exception is now returned on failure.
                return e;
            }

            return null;
        }

        private void CreatePlugins()
        {
            var catalog = new DirectoryCatalog(MEFCatalog);
            _container = new CompositionContainer(catalog);

            bool registratorDiscovered = false;
            try
            {
                var batch = new CompositionBatch();
                batch.AddPart(_interfacesPl);
                _container.Compose(batch);
                registratorDiscovered = true;
            }
            catch (CompositionException)
            {
                // thats OK if no plugin of this type, then move on trying to load others
                //throw new ArgumentException("Could not compose MEF Report View or Exception trapping strategy. " + Environment.NewLine + "Ensure that there exist one and only one assembly with export of IExceptionReportView and IExceptionTrappingStrategy in the runtime folder.");
            }

            if (!registratorDiscovered)
            {
                try
                {
                    var batch = new CompositionBatch();
                    batch.AddPart(_interfaces);
                    _container.Compose(batch);
                }
                catch (CompositionException ex)
                {
                    throw new ArgumentException("Could not compose MEF Report View or Exception trapping strategy. " + Environment.NewLine + "Ensure that there exist one and only one assembly with export of IExceptionReportView and IExceptionTrappingStrategy in the runtime folder.", ex);
                }
            }
        }

        private void InvokeDeliveryFailure(string message)
        {
            try
            {
                //failed to deliver exception display for user.
                if (ReportingUI)
                    Reportform.ShowDeliveryFailure(message);
            }
            catch (System.Exception ex)
            {
                //failed to show delivery failure... just log 
                ReportLogger.Instance.LogExceptionsDuringDelivery(
                    new System.Exception("Failed to show delivery exception failure dialog.", ex));
            }
        }
    }
}