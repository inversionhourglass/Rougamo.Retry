using Rougamo.Context;
using System.Threading.Tasks;

namespace Rougamo.Retry
{
    /// <summary>
    /// Basic retry logic
    /// </summary>
    public abstract class BaseRetryAttribute : RawMoAttribute
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
        public override ValueTask OnEntryAsync(MethodContext context)
        {
            OnEntry(context);

            return default;
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

#if NETSTANDARD2_1_OR_GREATER
        /// <inheritdoc/>
        public override async ValueTask OnExceptionAsync(MethodContext context)
        {
            if (_definition == null || context.ExceptionHandled || context.Exception == null) return;

            var exceptionContext = new ExceptionContext(context.Target, context.Method, context.Exception);
            if (_definition.Match(context.Exception))
            {
                context.RetryCount--;
                if (context.RetryCount > 0)
                {
                    await _definition.TemporaryFailedAsync(exceptionContext);
                    return;
                }
            }
            else
            {
                context.RetryCount = 0;
            }
            await _definition.UltimatelyFailedAsync(exceptionContext);
        }
#else
        /// <inheritdoc/>
        public override ValueTask OnExceptionAsync(MethodContext context)
        {
            OnException(context);

            return default;
        }
#endif

        /// <inheritdoc/>
        public override void OnSuccess(MethodContext context)
        {
            if (_definition == null) return;

            context.RetryCount = 0;
        }

        /// <inheritdoc/>
        public override ValueTask OnSuccessAsync(MethodContext context)
        {
            OnSuccess(context);

            return default;
        }

        /// <inheritdoc/>
        public sealed override void OnExit(MethodContext context)
        {
            
        }

        /// <inheritdoc/>
        public sealed override ValueTask OnExitAsync(MethodContext context)
        {
            return default;
        }
    }
}
