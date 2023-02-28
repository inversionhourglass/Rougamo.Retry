using Rougamo.Retry.Internal;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Rougamo.Retry
{
    internal static class ReflectionExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> _Ctors = new();

        public static object New(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (type == typeof(IRecordable)) return new NonRecordable();

            var ctor = _Ctors.GetOrAdd(type, t =>
            {
                var ctor = t.GetConstructor(new Type[0]);
                var lambda = Expression.Lambda<Func<object>>(Expression.New(ctor));
                return lambda.Compile();
            });

            return ctor();
        }
    }
}
