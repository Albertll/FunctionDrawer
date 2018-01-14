using System;

namespace FunctionDrawer
{
    internal class FunctionCalculator : IFunctionCalculator
    {
        public IOperation Operation { get; set; }
        public DoublePoint ScaleFactor { get; } = new DoublePoint(-7, -7);
        public DoublePoint Movement { get; set; } = new DoublePoint();

        public DoublePoint Scale
            => new DoublePoint(Math.Pow(2, ScaleFactor.X), Math.Pow(2, ScaleFactor.Y));

        public double GetXFromScreenX(double screenX)
            => screenX * Scale.X + Movement.X;

        public double GetScreenXFromX(double x)
            => (x - Movement.X) / Scale.X;

        public double GetYFromX(double x)
            => Operation.Result(x);

        public double GetScreenYFromY(double y)
            => -(y + Movement.Y) / Scale.Y;

        public double GetYFromScreenY(double screenY)
            => -screenY * Scale.Y - Movement.Y;

        //private double GetScreenYFromY2(double y) => -(y + _movement.Y) / Scale.Y;
    }
}