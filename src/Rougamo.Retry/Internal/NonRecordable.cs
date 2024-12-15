using System.Threading.Tasks;

namespace Rougamo.Retry.Internal
{
    internal class NonRecordable : IRecordable
    {
        public void TemporaryFailed(ExceptionContext context)
        {
        }

        public void UltimatelyFailed(ExceptionContext context)
        {
        }

#if NETSTANDARD2_1_OR_GREATER
        public ValueTask TemporaryFailedAsync(ExceptionContext context)
        {
            return default;
        }

        public ValueTask UltimatelyFailedAsync(ExceptionContext context)
        {
            return default;
        }
#endif
    }
}
