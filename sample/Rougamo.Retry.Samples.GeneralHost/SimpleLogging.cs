using Microsoft.Extensions.Logging;
using System;

namespace Rougamo.Retry.Samples.GeneralHost
{
    public class SimpleLogging : ISyncRecordableMatcher
    {
        private readonly ILogger _logger;

        public SimpleLogging(ILogger<SimpleLogging> logger)
        {
            _logger = logger;
        }

        public bool Match(Exception e) => e is SystemException;

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
