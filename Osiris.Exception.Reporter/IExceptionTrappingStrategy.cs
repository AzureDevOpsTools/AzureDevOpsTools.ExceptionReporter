using System;

namespace Inmeta.Exception.Reporter
{

    /// <summary>
    /// This interface represents the strategy for trapping exceptions.
    /// </summary>
    public interface IExceptionTrappingStrategy
    {
        /// <summary>
        /// Register all exception events which will automatically trigger a callback.
        /// </summary>
        /// <param name="callback">All caught exceptions shall be forwarded to the provided callback
        /// The bool shall indicate, if the system is terminating (true) or not (false).</param>
        void RegisterExceptionEvents(Action<System.Exception, bool> callback);

        /// <summary>
        /// Unregister events.
        /// </summary>
        void UnRegister();

    }
}