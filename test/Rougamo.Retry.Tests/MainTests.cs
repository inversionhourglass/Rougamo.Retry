using Rougamo.Retry.Tests.Impl;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rougamo.Retry.Tests
{
    public class MainTests
    {
        [Fact]
        public async Task OnceTest()
        {
            var counting = new Couting();
            var once = new Once();

            Mute(() => once.Sync(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();

            await Mute(() => once.Async(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();

            await Mute(async () => await once.VAsync(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();

            Mute(() => Once.StaticSync(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();

            await Mute(() => Once.StaticAsync(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();

            await Mute(async () => await Once.StaticVAsync(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();
        }

        [Fact]
        public async Task TwiceTest()
        {
            var counting = new Couting();
            var twice = new Twice();

            Mute(() => twice.Sync(counting));
            Assert.Equal(2, counting.Value);
            counting.Reset();

            await Mute(() => twice.Async(counting));
            Assert.Equal(2, counting.Value);
            counting.Reset();

            await Mute(async () => await Twice.VAsync(counting));
            Assert.Equal(2, counting.Value);
            counting.Reset();

            await Mute(async () => await Twice.RetrySucceed(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();
        }

        [Fact]
        public async Task SpecialTest()
        {
            var counting = new Couting();
            var special = new SpecialExceptions();

            Mute(() => special.Matched(counting));
            Assert.Equal(SpecialExceptions.RETRY_TIMES, counting.Value);
            counting.Reset();

            await Mute(() => special.MatchedAsync(counting));
            Assert.Equal(SpecialExceptions.RETRY_TIMES, counting.Value);
            counting.Reset();

            await Mute(async () => await SpecialExceptions.UnmatchedAsync(counting));
            Assert.Equal(0, counting.Value);
            counting.Reset();

            Mute(() => SpecialExceptions.MatchedOnceAsync(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();
        }

        [Fact]
        public async Task CustomTest()
        {
            var counting = new Couting();
            var custom = new Custom();

            Mute(() => custom.Matched(counting));
            Assert.Equal(RetryDefinition.RETRY_TIMES, counting.Value);
            counting.Reset();

            await Mute(() => Custom.MatchedAsync(counting));
            Assert.Equal(RetryDefinition.RETRY_TIMES, counting.Value);
            counting.Reset();

            await Mute(async () => await custom.UnmatchedAsync(counting));
            Assert.Equal(0, counting.Value);
            counting.Reset();

            Mute(() => Custom.MatchedOnceAsync(counting));
            Assert.Equal(1, counting.Value);
            counting.Reset();
        }

        private void Mute(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // ignore
            }
        }

        private async Task Mute(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch
            {
                // ignore
            }
        }
    }
}