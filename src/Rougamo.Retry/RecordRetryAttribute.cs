using Rougamo.Metadatas;
using Rougamo.Retry.Internal;
using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// Auto use <see cref="IRecordable"/> implementation to record exception
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [Advice(Feature.OnEntry | Feature.Retry)]
    [Optimization(ForceSync = ForceSync.All, MethodContext = Context.Omit.All)]
    public class RecordRetryAttribute : BaseRetryAttribute
    {
        /// <summary>
        /// Any exception retry once
        /// </summary>
        public RecordRetryAttribute() : this(1, typeof(Exception)) { }

        /// <summary>
        /// Any exception retry <paramref name="retryTimes"/> times
        /// </summary>
        public RecordRetryAttribute(int retryTimes) : this(retryTimes, typeof(Exception)) { }

        /// <summary>
        /// retry <paramref name="retryTimes"/> times if <see cref="IExceptionMatcher.Match(Exception)"/> return true
        /// </summary>
        public RecordRetryAttribute(int retryTimes, Type exceptionOrMatcherType)
        {
            IExceptionMatcher matcher;
            var recordable = (IRecordable)Resolver.Facatory(typeof(IRecordable));
            if (typeof(IExceptionMatcher).IsAssignableFrom(exceptionOrMatcherType))
            {
                matcher = (IExceptionMatcher)Resolver.Facatory(exceptionOrMatcherType);
            }
            else
            {
                matcher = new ExceptionMatcher(exceptionOrMatcherType);
            }
            _definition = new RecordableRetryDefinition(retryTimes, matcher, recordable);
        }

        /// <summary>
        /// retry <paramref name="retryTimes"/> times if the exception type is one of <paramref name="exceptionTypes"/> or subclass of <paramref name="exceptionTypes"/>
        /// </summary> 
        public RecordRetryAttribute(int retryTimes, params Type[] exceptionTypes)
        {
            var matcher = new ExceptionMatcher(exceptionTypes);
            var recordable = (IRecordable)Resolver.Facatory(typeof(IRecordable));
            _definition = new RecordableRetryDefinition(retryTimes, matcher, recordable);
        }

        /// <summary>
        /// <paramref name="retryDefType"/> must implement <see cref="IRetryDefinition"/>, retry <see cref="IRetryTimes.Times"/> if <see cref="IExceptionMatcher.Match(Exception)"/> return true.
        /// </summary>
        /// <param name="retryDefType"><see cref="IRetryDefinition"/></param>
        public RecordRetryAttribute(Type retryDefType)
        {
            var definition = (IRetryDefinition)Resolver.Facatory(retryDefType);
            var recordable = (IRecordable)Resolver.Facatory(typeof(IRecordable));
            _definition = new RecordableRetryDefinition(definition.Times, definition, recordable);
        }
    }
}
