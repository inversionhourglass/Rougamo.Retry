using System;
using System.Threading.Tasks;

namespace Rougamo.Retry.Internal
{
    internal class RecordableRetryDefinition : IRecordableRetryDefinition
    {
        private readonly IRecordableMatcher _recordableMatcher;

        public RecordableRetryDefinition(int retryTimes, IRecordableMatcher recordableMatcher)
        {
            Times = retryTimes;
            _recordableMatcher = recordableMatcher;
        }

        public RecordableRetryDefinition(int retryTimes, IExceptionMatcher matcher, IRecordable recordable)
        {
            Times = retryTimes;
            _recordableMatcher = new RecordableMatcher(matcher, recordable);
        }

        public int Times { get; }

        public bool Match(Exception e) => _recordableMatcher.Match(e);

        public void TemporaryFailed(ExceptionContext context) => _recordableMatcher.TemporaryFailed(context);

        public void UltimatelyFailed(ExceptionContext context) => _recordableMatcher.UltimatelyFailed(context);

#if NETSTANDARD2_1_OR_GREATER
        public ValueTask TemporaryFailedAsync(ExceptionContext context) => _recordableMatcher.TemporaryFailedAsync(context);

        public ValueTask UltimatelyFailedAsync(ExceptionContext context) => _recordableMatcher.UltimatelyFailedAsync(context);
#endif
    }
}
