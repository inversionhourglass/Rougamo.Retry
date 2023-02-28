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
    }
}
