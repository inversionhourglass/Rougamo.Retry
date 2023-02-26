using System;
using System.IO;
using System.Threading.Tasks;

namespace Rougamo.Retry.Tests.Impl
{
    internal class Custom
    {
        [Retry(typeof(DataRetryDefinition))]
        public void Matched(Couting couting)
        {
            couting.Increase();
            throw new InvalidOperationException().EnableRetry();
        }

        [Retry(typeof(DataRetryDefinition))]
        public async static Task MatchedAsync(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new IOException().EnableRetry();
        }

        [Retry(typeof(DataRetryDefinition))]
        public async ValueTask UnmatchedAsync(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new ArgumentException();
        }

        [Retry(typeof(DataRetryDefinition))]
        public static void MatchedOnceAsync(Couting couting)
        {
            couting.Increase();
            if (couting.Value == 0) throw new IOException().EnableRetry();

            throw new ArithmeticException();
        }
    }
}
