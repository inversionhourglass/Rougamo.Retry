using System;
using System.Threading.Tasks;

namespace Rougamo.Retry.Internal
{
    internal class RecordableMatcher : IRecordableMatcher
    {
        private readonly IExceptionMatcher _matcher;
        private readonly IRecordable _recordable;

        public RecordableMatcher(IExceptionMatcher matcher, IRecordable recordable)
        {
            _matcher = matcher;
            _recordable = recordable;
        }

        public bool Match(Exception e) => _matcher.Match(e);

        public void TemporaryFailed(ExceptionContext context) => _recordable.TemporaryFailed(context);

        public void UltimatelyFailed(ExceptionContext context) => _recordable.UltimatelyFailed(context);

#if NETSTANDARD2_1_OR_GREATER
        public ValueTask TemporaryFailedAsync(ExceptionContext context) => _recordable.TemporaryFailedAsync(context);

        public ValueTask UltimatelyFailedAsync(ExceptionContext context) => _recordable.UltimatelyFailedAsync(context);
#endif
    }
}
