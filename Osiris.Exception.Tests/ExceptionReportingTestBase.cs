using System.IO;
using NUnit.Framework;


namespace Osiris.Exception.Tests
{
    public class ExceptionReportingTestBase
    {
        protected static string WinformFolder = "";
        protected static string WPFFolder = "";

        public ExceptionReportingTestBase()
        {
            WinformFolder = TestContext.CurrentContext.TestDirectory;
            WPFFolder = TestContext.CurrentContext.TestDirectory;
            //Directory.CreateDirectory(WinformFolder);
            //Directory.CreateDirectory(WPFFolder);

        //    if (!File.Exists(WinformFolder + "Inmeta.Exception.Report.WinForm.dll"))
        //        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "Inmeta.Exception.Report.WinForm.dll"), WinformFolder + "Inmeta.Exception.Report.WinForm.dll");

        //    if (!File.Exists(WPFFolder + "Inmeta.Exception.Report.WPF.dll"))
        //        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "Inmeta.Exception.Report.WPF.dll"), WPFFolder + "Inmeta.Exception.Report.WPF.dll");
        //
        }

    }
}
