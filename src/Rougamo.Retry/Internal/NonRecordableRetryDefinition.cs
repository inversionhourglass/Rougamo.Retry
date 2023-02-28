using System;

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
    }
}
