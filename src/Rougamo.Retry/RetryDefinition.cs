using System;

namespace Rougamo.Retry
{
    /// <summary>
    /// <see cref="RetryDefinitionFactory"/> holder
    /// </summary>
    public static class RetryDefinition
    {
        internal static RetryDefinitionFactory Facatory = ReflectionExtensions.New;

        /// <summary>
        /// </summary>
        public static RetryDefinitionFactory Default { get; } = ReflectionExtensions.New;

        /// <summary>
        /// </summary>
        public static void Set(RetryDefinitionFactory factory) => Facatory = factory;
    }

    /// <summary>
    /// How to resolve <see cref="IRetryDefinition"/>
    /// </summary>
    public delegate IRetryDefinition RetryDefinitionFactory(Type type);
}
