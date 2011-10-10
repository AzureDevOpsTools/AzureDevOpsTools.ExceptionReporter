using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Osiris.Exception.Tests
{
    public class ExceptionReportingTestBase
    {
        protected static string WinformFolder = "";
        protected static string WPFFolder = "";

        public ExceptionReportingTestBase()
        {
            WinformFolder = Directory.GetCurrentDirectory() + @"\winform\";
            WPFFolder = Directory.GetCurrentDirectory() + @"\wpf\";
            Directory.CreateDirectory(WinformFolder);
            Directory.CreateDirectory(WPFFolder);

            if (!File.Exists(WinformFolder + "Inmeta.Exception.Report.WinForm.dll"))
                File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "Inmeta.Exception.Report.WinForm.dll"), WinformFolder + "Inmeta.Exception.Report.WinForm.dll");

            if (!File.Exists(WPFFolder + "Inmeta.Exception.Report.WPF.dll"))
                File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "Inmeta.Exception.Report.WPF.dll"), WPFFolder + "Inmeta.Exception.Report.WPF.dll");
        }

    }
}
