using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// Does the exception Matched
    /// </summary>
    public interface IExceptionMatcher
    {
        /// <summary>
        /// Determine whether the exception matches
        /// </summary>
        bool Match(Exception e);
    }
}
