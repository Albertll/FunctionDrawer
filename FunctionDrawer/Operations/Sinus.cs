using System;

namespace FunctionDrawer.Operations
{
    internal class Sinus : OneArgumentOperation
    {
        public Sinus(Operation right)
            : base(right) { }

        protected override double Evaluate(double x)
            => Math.Sin(Right.Result(x));

        public override string ToString()
            => $"sin({Right})";
    }
}