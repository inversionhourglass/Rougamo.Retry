using Microsoft.Extensions.Hosting;

namespace Rougamo.Retry.Samples.StaticAccessor
{
    internal class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Failed();
            await FailedAsync();
            var x = 0;
            StaticRetrySucceed(3, ref x);
            await StaticRetryFailedAsync();
        }

        [MuteException]
        [RecordRetry(3)]
        private void Failed()
        {
            throw new NotImplementedException();
        }

        [MuteException]
        [RecordRetry(3)]
        private async Task FailedAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }

        [MuteException]
        [RecordRetry(3)]
        private static void StaticRetrySucceed(int originValue, ref int value)
        {
            if (originValue == value)
            {
                value++;
                throw new ArgumentException();
            }
        }

        [MuteException]
        [RecordRetry(3)]
        private static async ValueTask StaticRetryFailedAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}
