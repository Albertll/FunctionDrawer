namespace FunctionDrawer.Operations
{
    internal sealed class Value : Operation
    {
        public Value(double value)
        {
            Value = value;
        }

        protected override double Evaluate(double x)
            => Value;
    }
}