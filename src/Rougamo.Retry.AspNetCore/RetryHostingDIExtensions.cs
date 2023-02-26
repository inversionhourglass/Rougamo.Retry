using Rougamo.Retry.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions
    /// </summary>
    public static class RetryHostingDIExtensions
    {
        /// <summary>
        /// Relace DefinitionFactory to global <see cref="IServiceProvider"/>
        /// </summary>
        public static IServiceCollection AddAspNetRetryFactory(this IServiceCollection services)
        {
            services.AddHostedService<RetryHostedService>();

            return services;
        }
    }
}
