using System;

namespace Rougamo.Retry
{
    internal class NonRecordableRetryDefinition : IRetryExceptionRecordable
    {
        private readonly IRetryDefinition _definition;

        public NonRecordableRetryDefinition(IRetryDefinition definition)
        {
            _definition = definition;
        }

        public NonRecordableRetryDefinition(int retryTimes, IExceptionMatcher matcher)
        {
            _definition = new MatcherRetryDefinition(retryTimes, matcher);
        }

        public int Times => _definition.Times;

        public bool Match(Exception e) => _definition.Match(e);

        public void TemporaryFailed(Exception e)
        {
        }

        public void UltimatelyFailed(Exception e)
        {
        }
    }
}
