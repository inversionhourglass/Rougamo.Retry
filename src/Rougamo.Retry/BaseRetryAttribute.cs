using Rougamo.Context;

namespace Rougamo.Retry
{
    /// <summary>
    /// Basic retry logic
    /// </summary>
    public abstract class BaseRetryAttribute : MoAttribute
    {
        /// <summary/>
        protected IRecordableRetryDefinition? _definition;

        /// <inheritdoc/>
        public override void OnEntry(MethodContext context)
        {
            if (_definition == null) return;

            context.RetryCount = _definition.Times + 1;
        }

        /// <inheritdoc/>
        public override void OnException(MethodContext context)
        {
            if (_definition == null || context.ExceptionHandled || context.Exception == null) return;

            var exceptionContext = new ExceptionContext(context.Target, context.Method, context.Exception);
            if (_definition.Match(context.Exception))
            {
                context.RetryCount--;
                if (context.RetryCount > 0)
                {
                    _definition.TemporaryFailed(exceptionContext);
                    return;
                }
            }
            else
            {
                context.RetryCount = 0;
            }
            _definition.UltimatelyFailed(exceptionContext);
        }

        /// <inheritdoc/>
        public override void OnSuccess(MethodContext context)
        {
            if (_definition == null) return;

            context.RetryCount = 0;
        }
    }
}
