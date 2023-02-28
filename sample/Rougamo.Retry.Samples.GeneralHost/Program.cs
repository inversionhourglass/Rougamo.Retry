using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Rougamo.Retry.Samples.GeneralHost
{
    internal class Program
    {
        static Task Main(string[] args)
        {
            return Host
                    .CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        services.AddRetryFactory();

                        services.AddHostedService<MainHostedService>();

                        services.AddTransient<SimpleLogging>();
                    })
                    .RunConsoleAsync();
        }
    }
}
