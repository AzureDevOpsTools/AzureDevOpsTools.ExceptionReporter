using System;

namespace WPFBasedExceptionReporter
{
    /// <summary>
    /// An exception that looks (as much as possible) like the exception passed to it. 
    /// Adds random text at end of stack-trace.
    /// </summary>
    [Serializable]
    public class RandomizedCopyException : Exception
    {
        private Exception _other;
        public RandomizedCopyException(Exception other)
            : base(other.Message, other)
        {
            _other = other;
        }

        public override string Source
        {
            get
            {
                return _other.Source;
            }
            set
            {
                _other.Source = value;
            }
        }

        public override string StackTrace
        {
            get
            {
                return _other.StackTrace + "\n\nSalt: " + new Random().NextDouble();
            }
        }

    }
}
