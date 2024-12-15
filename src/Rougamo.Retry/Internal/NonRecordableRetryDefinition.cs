using System;
using System.Threading.Tasks;

namespace Rougamo.Retry.Internal
{
    internal class NonRecordableRetryDefinition : IRecordableRetryDefinition
    {
        private readonly IRetryDefinition _definition;

        public NonRecordableRetryDefinition(IRetryDefinition definition)
        {
            _definition = definition;
        }

        public NonRecordableRetryDefinition(int retryTimes, IExceptionMatcher matcher)
        {
            _definition = new RetryDefinition(retryTimes, matcher);
        }

        public int Times => _definition.Times;

        public bool Match(Exception e) => _definition.Match(e);

        public void TemporaryFailed(ExceptionContext context)
        {
        }

        public void UltimatelyFailed(ExceptionContext context)
        {
        }

#if NETSTANDARD2_1_OR_GREATER
        public ValueTask TemporaryFailedAsync(ExceptionContext context)
        {
            return default;
        }

        public ValueTask UltimatelyFailedAsync(ExceptionContext context)
        {
            return default;
        }
#endif
    }
}
