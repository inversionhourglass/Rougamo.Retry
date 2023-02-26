using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Rougamo.Retry.AspNetCore
{
    internal class RetrySetProviderMiddleware
    {
        private readonly RequestDelegate _next;

        public RetrySetProviderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            ServiceProviderHolder.SetProvider(context.RequestServices);

            await _next(context);
        }
    }
}
