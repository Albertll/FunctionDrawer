namespace FunctionDrawer.Operations
{
    internal class Multiplication : TwoArgumentsOperation
    {
        public Multiplication(Operation left, Operation inner)
            : base(left, inner) { }

        protected override double Evaluate(double x)
            => Left.Result(x) * Right.Result(x);
    }
}