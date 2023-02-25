using System;
using System.IO;
using System.Threading.Tasks;

namespace Rougamo.Retry.Tests.Impl
{
    internal class SpecialExceptions
    {
        public const int RETRY_TIMES = 3;

        [Retry(RETRY_TIMES, typeof(InvalidOperationException), typeof(IOException))]
        public void Matched(Couting couting)
        {
            couting.Increase();
            throw new InvalidOperationException();
        }

        [Retry(RETRY_TIMES, typeof(InvalidOperationException), typeof(IOException))]
        public async Task MatchedAsync(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new IOException();
        }

        [Retry(RETRY_TIMES, typeof(InvalidOperationException), typeof(IOException))]
        public async static ValueTask UnmatchedAsync(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new ArgumentException();
        }

        [Retry(RETRY_TIMES, typeof(InvalidOperationException), typeof(IOException))]
        public static void MatchedOnceAsync(Couting couting)
        {
            couting.Increase();
            if (couting.Value == 0) throw new IOException();

            throw new ArithmeticException();
        }
    }
}
