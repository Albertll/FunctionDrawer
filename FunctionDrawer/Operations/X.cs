namespace FunctionDrawer.Operations
{
    internal sealed class X : Operation
    {
        protected override double Evaluate(double x)
            => x;

        public override string ToString()
            => "x";
    }
}