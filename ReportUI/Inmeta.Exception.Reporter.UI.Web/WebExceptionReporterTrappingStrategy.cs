using System;
using System.ComponentModel.Composition;
using System.Web;
using Inmeta.Exception.Reporter.UI.Web;

namespace Inmeta.Exception.Reporter.Web
{
    [Export(typeof(IExceptionTrappingStrategy))]
    public class WebExceptionReporterTrappingStrategy : IExceptionTrappingStrategy
    {
        public WebExceptionReporterTrappingStrategy()
        {
            MvcApplication.OnException += (() => _callback(HttpContext.Current.Error, true));
        }

        private Action<System.Exception, bool> _callback;
        
        public void RegisterExceptionEvents(Action<System.Exception, bool> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            _callback = callback;
        }

        public void UnRegister()
        {
            _callback = null;
        }
    
    }
}
