#if NETSTANDARD2_1_OR_GREATER
namespace Rougamo.Retry
{
    /// <summary>
    /// Inherits from <see cref="IRecordable"/> and implements sync methods
    /// </summary>
    public interface IAsyncRecordable : IRecordable
    {
        void IRecordable.TemporaryFailed(ExceptionContext context)
        {
            TemporaryFailedAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        void IRecordable.UltimatelyFailed(ExceptionContext context)
        {
            UltimatelyFailedAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
#endif
