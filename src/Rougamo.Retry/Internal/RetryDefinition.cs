using System;

namespace Rougamo.Retry.Internal
{
    internal class RetryDefinition : IRetryDefinition
    {
        private readonly IExceptionMatcher _matcher;

        public RetryDefinition(int retryTimes, IExceptionMatcher matcher)
        {
            Times = retryTimes;
            _matcher = matcher;
        }

        public RetryDefinition(int retryTimes, params Type[] exceptionTypes) :
            this(retryTimes, new ExceptionMatcher(exceptionTypes))
        {
        }

        public int Times { get; }

        public bool Match(Exception e) => _matcher.Match(e);
    }
}
