using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inmeta.Exception.Reporter
{
    public interface IExceptionHandler 
    {
        void OnException(System.Exception e, bool isTerminating);
        void Init(ExceptionHandlerSettings properites);
    }
}
