namespace Rougamo.Retry.Tests.Impl
{
    internal class Couting
    {
        public int Value { get; private set; } = -1;

        public void Increase()
        {
            Value++;
        }

        public void Reset()
        {
            Value = -1;
        }
    }
}
