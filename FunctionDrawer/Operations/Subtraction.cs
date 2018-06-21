namespace FunctionDrawer.Operations
{
    internal class Subtraction : TwoArgumentsOperation
    {
        public Subtraction(Operation left, Operation inner) 
            : base(left, inner) { }

        protected override double Evaluate(double x)
            => Left.Result(x) - Right.Result(x);
    }
}