namespace FunctionDrawer.Operations
{
    internal class Addition : TwoArgumentsOperation
    {
        public Addition(Operation left, Operation inner) 
            : base(left, inner) { }

        protected override double Evaluate(double x)
            => Left.Result(x) + Right.Result(x);
    }
}