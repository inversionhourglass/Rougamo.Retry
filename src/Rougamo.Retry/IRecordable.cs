using System.Threading.Tasks;

namespace Rougamo.Retry
{
    /// <summary>
    /// Get exception object after exception occurred
    /// </summary>
    public interface IRecordable
    {
        /// <summary>
        /// Just a temporary failure, there is a chance to try again
        /// </summary>
        void TemporaryFailed(ExceptionContext context);

        /// <summary>
        /// The number of retries has been used up, and finally failed, or the exception is not matched
        /// </summary>
        void UltimatelyFailed(ExceptionContext context);

#if NETSTANDARD2_1_OR_GREATER
        /// <inheritdoc cref="TemporaryFailed(ExceptionContext)"/>
        ValueTask TemporaryFailedAsync(ExceptionContext context);

        /// <inheritdoc cref="UltimatelyFailed(ExceptionContext)"/>
        ValueTask UltimatelyFailedAsync(ExceptionContext context);
#endif
    }
}
