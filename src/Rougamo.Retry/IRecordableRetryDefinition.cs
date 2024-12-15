namespace Rougamo.Retry
{
    /// <summary>
    /// Both <see cref="IRecordableMatcher"/> and <see cref="IRetryTimes"/>
    /// </summary>
    public interface IRecordableRetryDefinition : IRetryDefinition, IRecordableMatcher
    {
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Inherits from <see cref="IRecordableRetryDefinition"/> and implements async methods
    /// </summary>
    public interface ISyncRecordableRetryDefinition : IRecordableRetryDefinition, ISyncRecordable
    {
    }

    /// <summary>
    /// Inherits from <see cref="IRecordableRetryDefinition"/> and implements async methods
    /// </summary>
    public interface IAsyncRecordableRetryDefinition : IRecordableRetryDefinition, IAsyncRecordable
    {
    }
#endif
}
