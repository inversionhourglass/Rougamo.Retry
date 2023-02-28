using System;

namespace Rougamo.Retry.Internal
{
    internal class ExceptionMatcher : IExceptionMatcher
    {
        private readonly Type[] _exceptionTypes;

        public ExceptionMatcher(params Type[] exceptionTypes)
        {
            _exceptionTypes = exceptionTypes;
        }

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
