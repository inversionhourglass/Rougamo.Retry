using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// Get exception object after exception occurred
    /// </summary>
    public interface IRetryExceptionRecordable : IRetryDefinition
    {
        /// <summary>
        /// Just a temporary failure, there is a chance to try again
        /// </summary>
        void TemporaryFailed(Exception e);

        /// <summary>
        /// The number of retries has been used up, and finally failed, or the exception is not matched
        /// </summary>
        void UltimatelyFailed(Exception e);
    }
}
