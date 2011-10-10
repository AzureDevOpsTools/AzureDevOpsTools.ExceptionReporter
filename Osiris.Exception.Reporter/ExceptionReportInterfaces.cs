using System.ComponentModel.Composition;

namespace Inmeta.Exception.Reporter
{
    public class ExceptionReportInterfaces
    {
        [Import]
        public IExceptionReportView Reportform { get; private set; }

        [Import]
        public IExceptionTrappingStrategy TrappingStrategy { get; private set; }
    }
}