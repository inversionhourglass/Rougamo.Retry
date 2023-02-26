using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rougamo.Retry.AspNetCore
{
    internal class RetryHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public RetryHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = ServiceProviderDefinitionFactory.Get(_serviceProvider);
            Resolver.Set(factory);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
