using System;

namespace FunctionDrawer.Operations
{
    internal class Division : TwoArgumentsOperation
    {
        public Division(Operation left, Operation inner)
            : base(left, inner) { }

        protected override double Evaluate(double x)
        {
            if (Right.Result(x).Equals(0))
                throw new DivideByZeroException();

            return Left.Result(x) / Right.Result(x);
        }

        public override string ToString()
            => $"({Left}/{Right})";
    }
}