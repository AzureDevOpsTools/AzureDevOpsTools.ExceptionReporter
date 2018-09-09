using System;
using AzureDevOps.Exception.Service.Common;

using AzureDevOps.Exception.Reporter;
using NUnit.Framework;
using AzureDevOps.Exception.Tests;

namespace AzureDevOps.Exception.Tests
{
    
    public class ExceptionRegistrator_ReportingFormType_Test  : ExceptionReportingTestBase
    {
        [Ignore("Possibly outdated")]
        [Test]
        public void ExceptionRegistrator_CreateWinForm_As_Default()
        {
            new ExceptionRegistrator("winformLoader", false, new DefaultServiceSettings(), WinformFolder);
        }

        [Ignore("Possibly outdated")]
        [Test]
        public void ExceptionRegistrator_CreateWPF()
        {
            try
            {
                new ExceptionRegistrator("winformLoader", false, new DefaultServiceSettings(), WPFFolder);
            }
            catch(NullReferenceException ex)
            {
                //this is no WPF application register will fail. 
            }
        }
    }

}
