using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Inmeta.Exception.Reporter;

namespace NTServiceBasedExceptionReporter
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //disable GUI reporting.
    //        Osiris.Exception.Reporter.Properties.Settings.Default.SetUseReportingUI = false;
            
            //use winform to set register unhandled event handlers
  //          Osiris.Exception.Reporter.Properties.Settings.Default.SetReportingFormType = "";

            //register NT Service register.
//            ExceptionRegistrator.Register("SERVICE TEST APP");

        }

        protected override void OnStop()
        {
        }
    }
}
