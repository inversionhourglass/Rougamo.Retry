#if NETSTANDARD2_1_OR_GREATER
using System.Threading.Tasks;

namespace Rougamo.Retry
{
    /// <summary>
    /// Inherits from <see cref="IRecordable"/> and implements async methods
    /// </summary>
    public interface ISyncRecordable : IRecordable
    {
        ValueTask IRecordable.TemporaryFailedAsync(ExceptionContext context)
        {
            TemporaryFailed(context);

            return default;
        }

        ValueTask IRecordable.UltimatelyFailedAsync(ExceptionContext context)
        {
            UltimatelyFailed(context);

            return default;
        }
    }
}
#endif
