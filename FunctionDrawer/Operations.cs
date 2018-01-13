using System;

namespace FunctionDrawer
{
    public interface IOperation
    {
        double Result(double x);
    }

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

        public double Result(double x) => _resultFunc(x);

        protected abstract double Evaluate(double x);

        protected virtual bool HasVariable => (Right?.HasVariable ?? false) || (Left?.HasVariable ?? false);
    }

    internal sealed class X : Operation
    {
        protected override double Evaluate(double x) => x;

        protected override bool HasVariable => true;
    }

    internal sealed class Pi : Operation
    {
        protected override double Evaluate(double x) => Math.PI;
    }

    internal sealed class Value : Operation
    {
        public Value(double value)
        {
            Value = value;
        }

        protected override double Evaluate(double x) => Value;
    }

    internal abstract class OneArgumentOperation : Operation
    {
        protected OneArgumentOperation(Operation right)
            : base(null, right) { }
    }

    internal abstract class TwoArgumentsOperation : Operation
    {
        protected TwoArgumentsOperation(Operation left, Operation right)
            : base(left, right) { }
    }

    internal class Addition : TwoArgumentsOperation
    {
        public Addition(Operation left, Operation right) : base(left, right) { }

        protected override double Evaluate(double x)
        {
            return Left.Result(x) + Right.Result(x);
        }
    }

    internal class Subtraction : TwoArgumentsOperation
    {
        public Subtraction(Operation left, Operation right) : base(left, right) { }

        protected override double Evaluate(double x)
        {
            return Left.Result(x) - Right.Result(x);
        }
    }

    internal class Multiplication : TwoArgumentsOperation
    {
        public Multiplication(Operation left, Operation right) : base(left, right) { }

        protected override double Evaluate(double x)
        {
            return Left.Result(x) * Right.Result(x);
        }
    }

    internal class Division : TwoArgumentsOperation
    {
        public Division(Operation left, Operation right) : base(left, right) { }

        protected override double Evaluate(double x)
        {
            if (Right.Result(x).Equals(0))
                throw new DivideByZeroException();

            return Left.Result(x) / Right.Result(x);
        }
    }

    internal class Power : TwoArgumentsOperation
    {
        public Power(Operation left, Operation right) : base(left, right) { }

        protected override double Evaluate(double x)
        {
            return Math.Pow(Left.Result(x), Right.Result(x));
        }
    }

    internal class Abs : OneArgumentOperation
    {
        public Abs(Operation right) : base(right) { }

        protected override double Evaluate(double x)
        {
            return Math.Abs(Right.Result(x));
        }
    }

    internal class Log : OneArgumentOperation
    {
        public Log(Operation right) : base(right) { }

        protected override double Evaluate(double x)
        {
            return Math.Log(Right.Result(x));
        }
    }

    internal class Sinus : OneArgumentOperation
    {
        public Sinus(Operation right) : base(right) { }

        protected override double Evaluate(double x)
        {
            return Math.Sin(Right.Result(x));
        }
    }

    internal class Cosinus : OneArgumentOperation
    {
        public Cosinus(Operation right) : base(right) { }

        protected override double Evaluate(double x)
        {
            return Math.Cos(Right.Result(x));
        }
    }

    internal class Tangens : OneArgumentOperation
    {
        public Tangens(Operation right) : base(right) { }

        protected override double Evaluate(double x)
        {
            return Math.Tan(Right.Result(x));
        }
    }

    internal class Negative : OneArgumentOperation
    {
        public Negative(Operation right) : base(right) { }

        protected override double Evaluate(double x)
        {
            return -Right.Result(x);
        }
    }
}
