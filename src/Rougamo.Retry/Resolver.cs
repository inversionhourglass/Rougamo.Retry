using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// <see cref="ResolverFactory"/> holder
    /// </summary>
    public static class Resolver
    {
        internal static ResolverFactory Facatory = ReflectionExtensions.New;

        /// <summary>
        /// </summary>
        public static ResolverFactory Default => ReflectionExtensions.New;

        /// <summary>
        /// </summary>
        public static void Set(ResolverFactory factory) => Facatory = factory;
    }

    /// <summary>
    /// How to resolve object
    /// </summary>
    public delegate object ResolverFactory(Type type);
}
