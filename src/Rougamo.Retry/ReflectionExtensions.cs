using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Rougamo.Retry
{
    internal static class ReflectionExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<IRetryDefinition>> _Ctors = new();

        public static IRetryDefinition New(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (!typeof(IRetryDefinition).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IRetryDefinition)}", nameof(type));

            var ctor = _Ctors.GetOrAdd(type, t =>
            {
                var ctor = t.GetConstructor(new Type[0]);
                var lambda = Expression.Lambda<Func<IRetryDefinition>>(Expression.New(ctor));
                return lambda.Compile();
            });

            return ctor();
        }
    }
}
