using System;

namespace FunctionDrawer.Operations
{
    internal class Tangens : OneArgumentOperation
    {
        public Tangens(Operation right)
            : base(right) { }

        protected override double Evaluate(double x)
            => Math.Tan(Right.Result(x));
    }
}