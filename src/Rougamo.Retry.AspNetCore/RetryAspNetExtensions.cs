using Microsoft.AspNetCore.Http;
using Rougamo.Retry.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extensions
    /// </summary>
    public static class RetryAspNetExtensions
    {
        /// <summary>
        /// Use <see cref="RetrySetProviderMiddleware"/>, change DefinitionFactory to current <see cref="HttpContext.RequestServices"/>
        /// </summary>
        /// <param name="app"></param>
        public static void UseRetryFactory(this IApplicationBuilder app)
        {
            app.UseMiddleware<RetrySetProviderMiddleware>();
        }
    }
}
