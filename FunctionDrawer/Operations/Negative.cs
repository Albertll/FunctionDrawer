namespace FunctionDrawer.Operations
{
    internal class Negative : OneArgumentOperation
    {
        public Negative(Operation right)
            : base(right) { }

        protected override double Evaluate(double x)
            => -Right.Result(x);
    }
}