using System;

namespace FunctionDrawer.Operations
{
    internal class Log : OneArgumentOperation
    {
        public Log(Operation right)
            : base(right) { }

        protected override double Evaluate(double x)
            => Math.Log(Right.Result(x));

        public override string ToString()
            => $"log({Right})";
    }
}