using Rougamo.Retry.Internal;
using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// Re-execute method if the exception is matched
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryAttribute : BaseRetryAttribute
    {
        /// <summary>
        /// Any exception retry once
        /// </summary>
        public RetryAttribute() : this(1, typeof(Exception)) { }

        /// <summary>
        /// Any exception retry <paramref name="retryTimes"/> times
        /// </summary>
        public RetryAttribute(int retryTimes) : this(retryTimes, typeof(Exception)) { }

        /// <summary>
        /// retry <paramref name="retryTimes"/> times if <see cref="IExceptionMatcher.Match(Exception)"/> return true
        /// </summary>
        public RetryAttribute(int retryTimes, Type exceptionOrMatcherOrRecordableType)
        {
            if (typeof(IRecordableMatcher).IsAssignableFrom(exceptionOrMatcherOrRecordableType))
            {
                _definition = new RecordableRetryDefinition(retryTimes, (IRecordableMatcher)Resolver.Facatory(exceptionOrMatcherOrRecordableType));
            }
            else if (typeof(IExceptionMatcher).IsAssignableFrom(exceptionOrMatcherOrRecordableType))
            {
                _definition = new NonRecordableRetryDefinition(retryTimes, (IExceptionMatcher)Resolver.Facatory(exceptionOrMatcherOrRecordableType));
            }
            else
            {
                _definition = new NonRecordableRetryDefinition(new RetryDefinition(retryTimes, exceptionOrMatcherOrRecordableType));
            }
        }

        /// <summary>
        /// retry <paramref name="retryTimes"/> times if the exception type is one of <paramref name="exceptionTypes"/> or subclass of <paramref name="exceptionTypes"/>
        /// </summary> 
        public RetryAttribute(int retryTimes, params Type[] exceptionTypes)
        {
            _definition = new NonRecordableRetryDefinition(new RetryDefinition(retryTimes, exceptionTypes));
        }

        /// <summary>
        /// <paramref name="retryDefType"/> must implement <see cref="IRetryDefinition"/>, retry <see cref="IRetryTimes.Times"/> if <see cref="IExceptionMatcher.Match(Exception)"/> return true.
        /// </summary>
        /// <param name="retryDefType"><see cref="IRetryDefinition"/></param>
        public RetryAttribute(Type retryDefType)
        {
            var definition = (IRetryDefinition)Resolver.Facatory(retryDefType);
            _definition = definition is IRecordableRetryDefinition recordableDefinition ? recordableDefinition : new NonRecordableRetryDefinition(definition);
        }
    }
}
