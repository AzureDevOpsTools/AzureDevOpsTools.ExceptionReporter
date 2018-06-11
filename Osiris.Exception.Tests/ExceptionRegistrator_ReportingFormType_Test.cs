using System;
using Inmeta.Exception.Service.Common;

using Inmeta.Exception.Reporter;
using NUnit.Framework;
using Osiris.Exception.Tests;

namespace Inmeta.Exception.Tests
{
    
    public class ExceptionRegistrator_ReportingFormType_Test  : ExceptionReportingTestBase
    {

        [Test]
        public void ExceptionRegistrator_CreateWinForm_As_Default()
        {
            new ExceptionRegistrator("winformLoader", false, new DefaultServiceSettings(), WinformFolder);
        }

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
