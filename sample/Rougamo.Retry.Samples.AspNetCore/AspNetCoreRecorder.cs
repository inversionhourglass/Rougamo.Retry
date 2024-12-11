using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Rougamo.Retry.Samples.AspNetCore
{
    public class AspNetCoreRecorder : IRecordable
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public AspNetCoreRecorder(ILogger<AspNetCoreRecorder> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public void TemporaryFailed(ExceptionContext context)
        {
            _logger.LogWarning(context.Exception, $"[{_contextAccessor.HttpContext.TraceIdentifier}] error occured in {context.Method.Name}");
        }

        public void UltimatelyFailed(ExceptionContext context)
        {
            _logger.LogError(context.Exception, $"[{_contextAccessor.HttpContext.TraceIdentifier}] finally we are failed to execute {context.Method.Name}");
        }
    }
}
