using System;
using System.Reflection;

namespace Rougamo.Retry
{
    /// <summary>
    /// Exception context
    /// </summary>
    public class ExceptionContext
    {
        /// <summary>
        /// </summary>
        public ExceptionContext(object? target, MethodBase method, Exception exception)
        {
            Target = target;
            Method = method;
            Exception = exception;
        }

        /// <summary>
        /// Type instance, null if it is a static method
        /// </summary>
        public object? Target { get; }

        /// <summary>
        /// The method of being woven
        /// </summary>
        public MethodBase Method { get; }

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; }
    }
}
