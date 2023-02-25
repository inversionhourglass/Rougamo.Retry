using System;

namespace Rougamo.Retry.Tests.Impl
{
    internal class RetryDefinition : IRetryDefinition
    {
        public const int RETRY_TIMES = 5;

        public int Times => RETRY_TIMES;

        public bool Match(Exception e) => e.IsRetry();
    }

    internal static class ExceptionExtensions
    {
        public const string RETRY_KEY = "retry";

        public static TException EnableRetry<TException>(this TException exception) where TException : Exception
        {
            exception.Data[RETRY_KEY] = null;

            return exception;
        }

        public static bool IsRetry(this Exception exception)
        {
            return exception.Data.Contains(RETRY_KEY);
        }
    }
}
