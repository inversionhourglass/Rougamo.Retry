﻿using System;

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
            var eType = e.GetType();
            foreach (var type in _exceptionTypes)
            {
                if (type.IsAssignableFrom(eType)) return true;
            }

            return false;
        }
    }
}
