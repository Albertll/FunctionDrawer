using System.Drawing;

namespace FunctionDrawer
{
    internal class DoublePoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public DoublePoint() : this(0, 0) { }

        public DoublePoint(double x, double y)
        { X = x; Y = y; }

        public static implicit operator DoublePoint(Point point)
            => new DoublePoint(point.X, point.Y);

        public static DoublePoint operator +(DoublePoint doublePoint, DoublePoint point2)
            => new DoublePoint(doublePoint.X + point2.X, doublePoint.Y + point2.Y);

        public static DoublePoint operator -(DoublePoint doublePoint, Point point2)
            => new DoublePoint(doublePoint.X - point2.X, doublePoint.Y - point2.Y);

        public static DoublePoint operator -(DoublePoint doublePoint, double value)
            => new DoublePoint(doublePoint.X - value, doublePoint.Y - value);

        public static DoublePoint operator *(DoublePoint doublePoint, DoublePoint point2)
            => new DoublePoint(doublePoint.X * point2.X, doublePoint.Y * point2.Y);
    }
}