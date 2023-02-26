using System;
using System.Threading;

namespace Rougamo.Retry.AspNetCore
{
    internal static class ServiceProviderHolder
    {
        private static readonly AsyncLocal<WeakReference<IServiceProvider>> _Provider = new();

        public static IServiceProvider? GetProvider()
        {
            var provider = _Provider.Value;
            if (provider == null) return null;

            return provider.TryGetTarget(out var sp) ? sp : null;
        }

        public static void SetProvider(IServiceProvider provider)
        {
            _Provider.Value = new WeakReference<IServiceProvider>(provider);
        }
    }
}
