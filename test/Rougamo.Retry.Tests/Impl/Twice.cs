using System;
using System.Threading.Tasks;

namespace Rougamo.Retry.Tests.Impl
{
    internal class Twice
    {
        [Retry(2)]
        public void Sync(Couting couting)
        {
            couting.Increase();
            throw new InvalidOperationException();
        }

        [Retry(2)]
        public async Task Async(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new InvalidOperationException();
        }

        [Retry(2)]
        public async static ValueTask VAsync(Couting couting)
        {
            couting.Increase();
            await Task.Yield();
            throw new InvalidOperationException();
        }

        [Retry(2)]
        public async static Task<string> RetrySucceed(Couting couting)
        {
            couting.Increase();
            if (couting.Value == 0) throw new Exception();

            await Task.Yield();

            return "ok";
        }
    }
}
