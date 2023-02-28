namespace Rougamo.Retry
{
    /// <summary>
    /// Define what kind of exception needs to be retried
    /// </summary>
    public interface IRetryTimes
    {
        /// <summary>
        /// Retry times
        /// </summary>
        int Times { get; }
    }
}
