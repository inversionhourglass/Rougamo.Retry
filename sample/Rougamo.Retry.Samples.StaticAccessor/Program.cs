using DependencyInjection.StaticAccessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rougamo.Retry.Samples.StaticAccessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Resolver.Set(t => PinnedScope.ScopedServices!.GetRequiredService(t));

            HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder(args);

            applicationBuilder.UsePinnedScopeServiceProvider();

            applicationBuilder.Services.AddHostedService<Worker>();
            applicationBuilder.Services.AddTransient<IRecordable, SimpleLogging>();

            IHost host = applicationBuilder.Build();
            host.Run();
        }
    }
}
