using System;

namespace FunctionDrawer.Operations
{
    internal abstract class Operation : IOperation
    {
        protected double Value;

        protected Operation Left;
        protected Operation Right;

        private readonly Func<double, double> _resultFunc;

        protected Operation(Operation left = null, Operation right = null)
        {
            Left = left;
            Right = right;

            if (!HasVariable)
            {
                Value = Evaluate(0);
                _resultFunc = _ => Value;
            }
            else
                _resultFunc = Evaluate;
        }

        public double Result(double x)
            => _resultFunc(x);

        protected abstract double Evaluate(double x);

        protected bool HasVariable
            => (Right?.HasVariable ?? false) || (Left?.HasVariable ?? false) || this is X;
    }
}
