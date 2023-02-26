using System;

namespace Rougamo.Retry.GeneralHost
{
    internal static class ServiceProviderDefinitionFactory
    {
        public static RetryDefinitionFactory Get(IServiceProvider serviceProvider)
        {
            return type =>
            {
                var obj = serviceProvider.GetService(type);
                return obj == null ? RetryDefinition.Default(type) : (IRetryDefinition)obj;
            };
        }
    }
}
