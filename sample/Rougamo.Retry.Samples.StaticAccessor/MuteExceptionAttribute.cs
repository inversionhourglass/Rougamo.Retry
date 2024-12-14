using Rougamo.Context;
using Rougamo.Metadatas;

namespace Rougamo.Retry.Samples.StaticAccessor
{
    [Advice(Feature.ExceptionHandle)]
    [Lifetime(Lifetime.Singleton)]
    internal class MuteExceptionAttribute : MoAttribute
    {
        public override void OnException(MethodContext context)
        {
            context.HandledException(this, null!);
        }
    }
}
