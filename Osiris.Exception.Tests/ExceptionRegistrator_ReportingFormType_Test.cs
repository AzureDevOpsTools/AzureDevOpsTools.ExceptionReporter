using System;
using System.IO;
using Inmeta.Exception.Service.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inmeta.Exception.Reporter;
using Fasterflect;
using Osiris.Exception.Tests;

namespace Inmeta.Exception.Tests
{
    [TestClass]
    public class ExceptionRegistrator_ReportingFormType_Test  : ExceptionReportingTestBase
    {

        [TestMethod]
        public void ExceptionRegistrator_CreateWinForm_As_Default()
        {
            new ExceptionRegistrator("winformLoader", false, new DefaultServiceSettings(), WinformFolder);
        }

        [TestMethod]
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
