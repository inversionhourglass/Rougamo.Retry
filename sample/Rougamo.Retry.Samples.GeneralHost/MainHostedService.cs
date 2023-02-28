using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rougamo.Retry.Samples.GeneralHost
{
    internal class MainHostedService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Failed();
                await Console.Out.WriteLineAsync($"{nameof(Failed)} execute succeed");
            }
            catch
            {
                await Console.Out.WriteLineAsync($"{nameof(Failed)} execute failed");
            }

            try
            {
                await FailedAsync();
                await Console.Out.WriteLineAsync($"{nameof(FailedAsync)} execute succeed");
            }
            catch
            {
                await Console.Out.WriteLineAsync($"{nameof(FailedAsync)} execute failed");
            }

            try
            {
                int v1, v2;
                v1 = v2 = 1;
                StaticRetrySucceed(v1, ref v2);
                await Console.Out.WriteLineAsync($"{nameof(StaticRetrySucceed)} execute succeed");
            }
            catch
            {
                await Console.Out.WriteLineAsync($"{nameof(StaticRetrySucceed)} execute failed");
            }

            try
            {
                await StaticRetryFailedAsync();
                await Console.Out.WriteLineAsync($"{nameof(StaticRetryFailedAsync)} execute succeed");
            }
            catch
            {
                await Console.Out.WriteLineAsync($"{nameof(StaticRetryFailedAsync)} execute failed");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        [Retry(3, typeof(SimpleLogging))]
        private void Failed()
        {
            throw new NotImplementedException();
        }

        [Retry(3, typeof(SimpleLogging))]
        private async Task FailedAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }

        [Retry(3, typeof(SimpleLogging))]
        private static void StaticRetrySucceed(int originValue, ref int value)
        {
            if (originValue == value)
            {
                value++;
                throw new ArgumentException();
            }
        }

        [Retry(3, typeof(SimpleLogging))]
        private static async ValueTask StaticRetryFailedAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}
