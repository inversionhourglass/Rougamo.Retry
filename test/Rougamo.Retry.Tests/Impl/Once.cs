using System;
using System.Threading.Tasks;

namespace Rougamo.Retry.Tests.Impl
{
    internal class Once
    {
        [Retry]
        public void Sync(Couting couting)
        {
            couting.Increase();
            throw new InvalidOperationException();
        }

        [Retry]
        public async Task Async(Couting couting)
        {
            await Task.Yield();
            couting.Increase();
            throw new InvalidOperationException();
        }

        [Retry]
        public async ValueTask VAsync(Couting couting)
        {
            await Task.Yield();
            couting.Increase();
            throw new InvalidOperationException();
        }

        [Retry]
        public static void StaticSync(Couting couting)
        {
            couting.Increase();
            throw new InvalidOperationException();
        }

        [Retry]
        public async static Task StaticAsync(Couting couting)
        {
            await Task.Yield();
            couting.Increase();
            throw new InvalidOperationException();
        }

        [Retry]
        public async static ValueTask StaticVAsync(Couting couting)
        {
            await Task.Yield();
            couting.Increase();
            throw new InvalidOperationException();
        }
    }
}
