namespace Rougamo.Retry
{
    /// <summary>
    /// Define what exceptions are matched and how exceptions should be recorded
    /// </summary>
    public interface IRecordableMatcher : IExceptionMatcher, IRecordable
    {
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Inherits from <see cref="IRecordableMatcher"/> and implements async methods
    /// </summary>
    public interface ISyncRecordableMatcher : IRecordableMatcher, ISyncRecordable
    {
    }

    /// <summary>
    /// Inherits from <see cref="IRecordableMatcher"/> and implements sync methods
    /// </summary>
    public interface IAsyncRecordableMatcher : IRecordableMatcher, IAsyncRecordable
    {
    }
#endif
}
