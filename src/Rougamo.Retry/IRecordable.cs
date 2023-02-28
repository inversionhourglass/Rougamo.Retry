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
    }
}
