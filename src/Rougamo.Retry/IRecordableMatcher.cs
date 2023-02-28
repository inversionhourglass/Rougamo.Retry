using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// Define what exceptions are matched and how exceptions should be recorded
    /// </summary>
    public interface IRecordableMatcher : IExceptionMatcher, IRecordable
    {
    }
}
