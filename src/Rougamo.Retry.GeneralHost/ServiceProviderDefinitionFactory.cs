using System;

namespace Rougamo.Retry.GeneralHost
{
    internal static class ServiceProviderDefinitionFactory
    {
        public static ResolverFactory Get(IServiceProvider serviceProvider)
        {
            return type =>
            {
                var obj = serviceProvider.GetService(type);
                return obj ?? Resolver.Default(type);
            };
        }
    }
}
