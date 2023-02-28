namespace Rougamo.Retry
{
    /// <summary>
    /// Both <see cref="IRecordableMatcher"/> and <see cref="IRetryTimes"/>
    /// </summary>
    public interface IRecordableRetryDefinition : IRetryDefinition, IRecordableMatcher
    {
    }
}
