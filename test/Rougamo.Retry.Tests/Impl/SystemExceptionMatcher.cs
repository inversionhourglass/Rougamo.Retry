using System;

namespace Rougamo.Retry.Tests.Impl
{
    internal class SystemExceptionMatcher : IExceptionMatcher
    {
        public bool Match(Exception e) => e is SystemException;
    }
}
