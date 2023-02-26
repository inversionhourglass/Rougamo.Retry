using System;

namespace Rougamo.Retry
{
    internal class MatcherRetryDefinition : IRetryDefinition
    {
        private readonly IExceptionMatcher _matcher;

        public MatcherRetryDefinition(int retryTimes, IExceptionMatcher matcher)
        {
            Times = retryTimes;
            _matcher = matcher;
        }

        public int Times { get; }

        public bool Match(Exception e) => _matcher.Match(e);
    }
}
