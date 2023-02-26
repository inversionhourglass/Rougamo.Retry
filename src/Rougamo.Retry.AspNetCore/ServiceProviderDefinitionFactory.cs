using System;

namespace Rougamo.Retry.AspNetCore
{
    internal static class ServiceProviderDefinitionFactory
    {
        public static RetryDefinitionFactory Get(IServiceProvider serviceProvider)
        {
            return type =>
            {
                object obj;
                try
                {
                    var provider = ServiceProviderHolder.GetProvider() ?? serviceProvider;
                    obj = provider.GetService(type);
                }
                catch
                {
                    obj = serviceProvider.GetService(type);
                }
                return obj == null ? RetryDefinition.Default(type) : (IRetryDefinition)obj;
            };
        }
    }
}
