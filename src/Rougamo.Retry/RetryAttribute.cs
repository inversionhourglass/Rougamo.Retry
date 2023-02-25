using Rougamo.Context;
using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// Re-execute method if the exception is matched
    /// </summary>
    public class RetryAttribute : MoAttribute
    {
        private readonly IRetryDefinition _retryDefinition;

        /// <summary>
        /// Any exception retry once
        /// </summary>
        public RetryAttribute() : this(1, typeof(Exception)) { }

        /// <summary>
        /// Any exception retry <paramref name="retryTimes"/> times
        /// </summary>
        public RetryAttribute(int retryTimes) : this(retryTimes, typeof(Exception)) { }

        /// <summary>
        /// retry <paramref name="retryTimes"/> times if the exception type is one of <paramref name="exceptionTypes"/> or subclass of <paramref name="exceptionTypes"/>
        /// </summary>
        public RetryAttribute(int retryTimes, params Type[] exceptionTypes)
        {
            _retryDefinition = new ExceptionRetryDefinition(retryTimes, exceptionTypes);
        }

        /// <summary>
        /// <paramref name="retryDefType"/> must implement <see cref="IRetryDefinition"/>, retry <see cref="IRetryDefinition.Times"/> if <see cref="IRetryDefinition.Match(Exception)"/> return true.
        /// </summary>
        /// <param name="retryDefType"><see cref="IRetryDefinition"/></param>
        public RetryAttribute(Type retryDefType)
        {
            _retryDefinition = retryDefType.New();
        }

        /// <inheritdoc/>
        public override void OnEntry(MethodContext context)
        {
            context.RetryCount = _retryDefinition.Times + 1;
        }

        /// <inheritdoc/>
        public override void OnException(MethodContext context)
        {
            if (!context.ExceptionHandled && context.Exception != null)
            {
                if (_retryDefinition.Match(context.Exception))
                {
                    context.RetryCount--;
                }
                else
                {
                    context.RetryCount = 0;
                }
            }
        }

        /// <inheritdoc/>
        public override void OnSuccess(MethodContext context)
        {
            context.RetryCount = 0;
        }
    }
}
