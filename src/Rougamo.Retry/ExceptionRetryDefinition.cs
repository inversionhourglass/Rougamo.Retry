using System;

namespace Rougamo.Retry
{
    internal class ExceptionRetryDefinition : IRetryDefinition
    {
        private readonly Type[] _exceptionTypes;

        public ExceptionRetryDefinition(int retryTimes, params Type[] exceptionTypes)
        {
            Times = retryTimes;
            _exceptionTypes = exceptionTypes;
        }

        public int Times { get; }

        public bool Match(Exception e)
        {
            foreach (var type in _exceptionTypes)
            {
                if (type.IsAssignableFrom(e.GetType())) return true;
            }

            return false;
        }
    }
}
