using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.Contracts;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
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
            MEFCatalog = mefCatalog ?? AppDomain.CurrentDomain.RelativeSearchPath;
            Register();
        }

        private  CompositionContainer _container;

        private  readonly object _syncObject = new object();

        private  object PreviousException { get; set; }
        public  bool ReportingUI { get; set; }
        private readonly ExceptionReportInterfaces _interfaces = new ExceptionReportInterfaces();
        private string MEFCatalog { get;set;}

        /// <summary>
        /// The application name.
        /// In nemo project this is HARD CODED TO "Instructor", should be able to report from another application name.
        /// </summary>
        private string ApplicationName { get; set; }

        /// <summary>
        /// Get Reporter, SAME AS NEMO, but more robust.
        /// </summary>
        private static string Reporter
        {
            get
            {
                try
                {
                    //same as NEMO, but with exception handling.
                    var currentNtUser = WindowsIdentity.GetCurrent();
                    return (currentNtUser != null) ? currentNtUser.Name : "n/a";
                }
                catch (SecurityException privEx)
                {
                    const string temp = "Insufficient privileges to extract the current user.";

                    ReportLogger.Instance.LogExceptionsDuringDelivery(
                        new System.Exception(temp, privEx));

                    return temp;
                }
            }
        }

        /// <summary>
        /// Get VERSION, SAME AS NEMO, but more robust.
        /// </summary>
        private static string Version
        {
            get
            {
                string version;
                try
                {
                    //SAME AS NEMO, but with exception handling
                    version =
                        Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch (System.Exception privEx)
                {
                    version =
                        "Insufficient privileges to extract the correct assembly version";
                    ReportLogger.Instance.LogExceptionsDuringDelivery(
                        new System.Exception(version, privEx));
                }

                return version;
            }
        }

        /// <summary>
        /// The exception beeing reported.
        /// </summary>
        private  System.Exception TheException { get; set; }

        internal  IExceptionReportView Reportform { get { return _interfaces.Reportform; } }

        internal  IExceptionTrappingStrategy TrappingStrategy { get { return _interfaces.TrappingStrategy; } }

        /// <summary>
        /// Call this to register the excpetion trapper. 
        /// This function should be the first function called in your application, it MUST be called before any forms are created. 
        /// Good  practice is to call it before Application.Run().
        /// </summary>
        private void Register()
        {
            Contract.Ensures(_interfaces.Reportform != null);
            Contract.Ensures(_interfaces.TrappingStrategy != null);

            //this code does not follow best practice in terms of compile time type checking. 
            //But this library will be used at the very low end of the component stack and we want 
            //to limit the amount of configuration bubbling up to application level to the minimum. 

            //get the registered form type.
            CreateViewAndTrapper();

            //make the Trapper register unhandled exceptions. 
            //Since this is done differently depending on application type,
            //The delegate provided is the event to call when an excpetion occurs.
            TrappingStrategy.RegisterExceptionEvents(OnException);

            ReportLogger.Instance.LogExceptionReporterInfo("Registered with Trapper = " + TrappingStrategy + ", View = " + Reportform);
        }

        private  void OnException(System.Exception e, bool isTerminating)
        {
            //only report one exception at the time.
            ReportLogger.Instance.LogExceptionReporterInfo("Received exception (isTerminating = " + isTerminating + ")");

            lock (_syncObject)
            {
                //avoid recursive reporting since we have registered both Main Form and AppDomain with unhandled exceptions
                if (PreviousException != null && e != null && PreviousException.ToString() == e.ToString())
                {
                    ReportLogger.Instance.LogExceptionReporterInfo(
                        new System.Exception("Trying to report on the same excpetion.", e).ToString());
                    //same as previous
                    return;
                }

                PreviousException = e;

                TheException = e;
                try
                {
                    //use ExceptionRegistrator.UseReportGUI to control if UI is to be used or not.
                    if (!ReportingUI)
                    {
                        //Call OnPost directly
                        OnPost("Exception reported w/o description");
                        return;
                    }

                    //Same as NEMO project:
                    var errorText = CreateExceptionText(e);

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
                            new System.Exception("Failed to show exception report.",
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

        private  void OnCancel(string description)
        {
            //send cancel report exception to log
            var report =
                new TFSExceptionReport(
                    ApplicationName,
                    Reporter,
                    Reporter,
                    TheException,
                    Version,
                    description ?? "Exception reported w/o description");

            ReportLogger.Instance.LogUnDeliveredExceptions(report);
        }

        private  void OnPost(string description)
        {
            try
            {
                //create exception entity
                var report = new TFSExceptionReport
                    (
                    ApplicationName,
                    Reporter,
                    Reporter, TheException,
                    Version,
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

        private System.Exception Post(TFSExceptionReport report)
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


        private  void InvokeDeliveryFailure(string message)
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

        /// <summary>
        /// Creates a string formed by exception and inner exceptions.
        /// It is the most inner exception that matters so I place it in front
        /// This code is the same as from Nemo project, but more robust.
        /// </summary>
        private static string CreateExceptionText(System.Exception e)
        {
            Contract.Requires(e != null);

            var errorText = new StringBuilder();

            errorText.Append(FormStringFromException(e));

            while (e.InnerException != null)
            {
                e = e.InnerException;
                errorText.Insert(0, FormStringFromException(e));
            }

            return errorText.ToString();
        }

        private static string FormStringFromException(System.Exception ex)
        {
            return string.Format("{0} at: {1}" + Environment.NewLine, ex.Message, ex.StackTrace ?? "Empty stack trace");
        }
        
        public  ServiceSettings ServiceSettings { get; private set; }

        private  void CreateViewAndTrapper()
        {
            try
            {
                var catalog = new DirectoryCatalog(MEFCatalog);
                _container = new CompositionContainer(catalog);
                var batch = new CompositionBatch();
                batch.AddPart(_interfaces);
                _container.Compose(batch);
            }
            catch (CompositionException)
            {
                throw new ArgumentException("Could not compose MEF Report View or Exception trapping strategy. " + Environment.NewLine +"Ensure that there exist one and only one assembly with export of IExceptionReportView and IExceptionTrappingStrategy in the runtime folder.");
            }
        }
    }
}