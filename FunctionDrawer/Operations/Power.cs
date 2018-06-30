using System;

namespace FunctionDrawer.Operations
{
    internal class Power : TwoArgumentsOperation
    {
        public Power(Operation left, Operation inner)
            : base(left, inner) { }

        protected override double Evaluate(double x)
            => Math.Pow(Left.Result(x), Right.Result(x));

        public override string ToString()
            => $"({Left}^{Right})";
    }
}