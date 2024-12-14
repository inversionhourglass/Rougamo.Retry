using Microsoft.Extensions.Logging;

namespace Rougamo.Retry.Samples.StaticAccessor
{
    public class SimpleLogging(ILogger<SimpleLogging> logger) : IRecordable
    {
        private readonly ILogger _logger = logger;

        public void TemporaryFailed(ExceptionContext context)
        {
            _logger.LogWarning(context.Exception, $"error occured in {context.Method.Name}");
        }

        public void UltimatelyFailed(ExceptionContext context)
        {
            _logger.LogError(context.Exception, $"finally we are failed to execute {context.Method.Name}");
        }
    }
}
