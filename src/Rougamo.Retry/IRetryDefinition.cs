namespace Rougamo.Retry
{
    /// <summary>
    /// Define what kind of exception needs to be retried
    /// </summary>
    public interface IRetryDefinition : IExceptionMatcher
    {
        /// <summary>
        /// Retry times
        /// </summary>
        int Times { get; }
    }
}
