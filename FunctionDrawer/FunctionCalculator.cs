using System;
using System.Runtime.CompilerServices;
using FunctionDrawer.Operations;

namespace FunctionDrawer
{
    internal class FunctionCalculator : IFunctionCalculator
    {
        public DoublePoint ScaleFactor { get; } = new DoublePoint(-7, -7);
        public DoublePoint Movement { get; set; } = new DoublePoint();
        
        public DoublePoint Scale
            => new DoublePoint(Math.Pow(2, ScaleFactor.X), Math.Pow(2, ScaleFactor.Y));

        public double GetXFromScreenX(double screenX)
            => screenX * Scale.X + Movement.X;

        public double GetYFromScreenY(double screenY)
            => -screenY * Scale.Y - Movement.Y;

        public double GetScreenXFromX(double x)
            => (x - Movement.X) / Scale.X;
        
        public double GetScreenYFromY(double y)
            => -(y + Movement.Y) / Scale.Y;
        
        public double GetYFromX(IOperation operation, double x)
            => operation.Result(x);
    }
}