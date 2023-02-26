using System;
using System.IO;
using System.Threading.Tasks;

namespace Rougamo.Retry.Tests.Impl
{
    internal class Matcher
    {
        public const int RETRY_TIMES = 3;

        [Retry(RETRY_TIMES, typeof(SystemExceptionMatcher))]
        public static void Matched(Couting couting)
        {
            couting.Increase();
            throw new InvalidCastException();
        }

        [Retry(RETRY_TIMES, typeof(SystemExceptionMatcher))]
        public async Task MatchedAsync(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new InvalidDataException();
        }

        [Retry(RETRY_TIMES, typeof(SystemExceptionMatcher))]
        public static async ValueTask UnmatchedAsync(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new Exception();
        }

        [Retry(RETRY_TIMES, typeof(SystemExceptionMatcher))]
        public void MatchedOnceAsync(Couting couting)
        {
            couting.Increase();
            if (couting.Value == 0) throw new IOException();

            throw new Exception();
        }
    }
}
