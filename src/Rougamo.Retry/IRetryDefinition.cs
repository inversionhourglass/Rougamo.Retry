namespace Rougamo.Retry
{
    /// <summary>
    /// Define what kind of exception needs to be retried and how many times to retry
    /// </summary>
    public interface IRetryDefinition : IExceptionMatcher, IRetryTimes
    {
    }
}
