using System;

namespace FunctionDrawer.Operations
{
    internal class Cosinus : OneArgumentOperation
    {
        public Cosinus(Operation right)
            : base(right) { }

        protected override double Evaluate(double x)
            => Math.Cos(Right.Result(x));
    }
}