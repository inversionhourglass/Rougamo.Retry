using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// Define what kind of exception needs to be retried and how many times to retry
    /// </summary>
    public interface IRetryDefinition
    {
        /// <summary>
        /// Retry times
        /// </summary>
        int Times { get; }

        /// <summary>
        /// Determine whether the exception matches
        /// </summary>
        bool Match(Exception e);
    }
}
