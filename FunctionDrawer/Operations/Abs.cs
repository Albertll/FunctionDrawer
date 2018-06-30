using System;

namespace FunctionDrawer.Operations
{
    internal class Abs : OneArgumentOperation
    {
        public Abs(Operation right)
            : base(right) { }

        protected override double Evaluate(double x) 
            => Math.Abs(Right.Result(x));

        public override string ToString()
            => $"abs({Right})";
    }
}