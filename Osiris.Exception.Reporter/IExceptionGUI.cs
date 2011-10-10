using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osiris.Exception.Reporter
{
    /// <summary>
    /// The command Interface
    /// </summary>
    public interface IExceptionGUI
    {
       
        void DisplayException();
        void PutException(string description);
        void Cancel();        
        
    }
    
}
