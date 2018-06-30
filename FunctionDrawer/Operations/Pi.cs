using System;

namespace FunctionDrawer.Operations
{
    internal sealed class Pi : Operation
    {
        protected override double Evaluate(double x)
            => Math.PI;

        public override string ToString()
            => "PI";
    }
}