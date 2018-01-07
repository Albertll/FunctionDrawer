using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FunctionDrawer.Properties;

namespace FunctionDrawer
{
    internal class ViewModel : NotifyPropertyChanged
    {
        #region Fields

        private readonly IList<int> _currentPoints = new List<int>();
        private string _errorMessage;
        private string _function;

        #endregion

        #region Properties

        internal IOperation Operation { get; set; }
        internal int Height { get; set; }
        internal int Width { get; set; }
        internal Color BackgroundColor { get; set; }
        private DoublePoint _scaleShift  = new DoublePoint(-7, -7);
        private DoublePoint _movement  = new DoublePoint();
        private DoublePoint _moveStartPoint;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public string Function
        {
            get { return _function; }
            set
            {
                _function = value;
                EvaluateOperation(value);
                OnPropertyChanged();
            }
        }

        #endregion

        public ViewModel()
        {
            Function = "x";
        }

        internal void EvaluateOperation(string input)
        {
            IOperation operation = null;
            try
            {
                operation = new ExpressionEvaluator().Evaluate(input);
                ErrorMessage = string.Empty;
            }
            catch (ArgumentException)
            {
                ErrorMessage = "XMissing argument";
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(Resources.Error, ex.Message);
            }
            finally
            {
                Operation = operation;
            }
        }

        public void StartMoving()
        {
            _moveStartPoint = _movement;
        }

        public void MoveScreen(int x, int y)
        {
            _movement = _moveStartPoint + new DoublePoint(x, y) * Scale;
            OnPropertyChanged("Movement");
        }

        private void ClearOldPoints(DirectBitmap dBitmap)
        {
            foreach (var point in _currentPoints)
                dBitmap[point] = BackgroundColor.ToArgb();

            _currentPoints.Clear();
        }

        private void Draw(DirectBitmap dBitmap, int x, int y, Color? color = null)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            var point = x + Width * y;
            dBitmap[point] = (color ?? Color.Black).ToArgb();
            _currentPoints.Add(point);
        }

        private void DrawCoordinateSystem(DirectBitmap dBitmap)
        {
            const int arrowWidth = 5;
            const int arrowHeight = 10;
            const int borderSpace = 10;

            Action<int, int> setPoint = (x, y) => Draw(dBitmap, x, y, Color.Blue);

            // vertical line
            var lineX = GetScreenXFromX(0);
            lineX = Math.Min(Math.Max(lineX, borderSpace * 2), Width - borderSpace * 2);
            DrawLine(setPoint, lineX, borderSpace + arrowHeight, lineX, Height);
            // arrow
            DrawLine(setPoint, lineX - arrowWidth, borderSpace + arrowHeight, lineX + arrowWidth, borderSpace + arrowHeight);
            DrawLine(setPoint, lineX - arrowWidth, borderSpace + arrowHeight, lineX, borderSpace);
            DrawLine(setPoint, lineX + arrowWidth, borderSpace + arrowHeight, lineX, borderSpace);

            // horizontal line
            var lineY = GetScreenYFromY(0);
            lineY = Math.Min(Math.Max(lineY, borderSpace * 2), Height - borderSpace * 2);
            DrawLine(setPoint, borderSpace, lineY, Width - borderSpace - arrowHeight, lineY);
            // arrow
            DrawLine(setPoint, Width - borderSpace - arrowHeight, lineY - arrowWidth, Width - borderSpace - arrowHeight, lineY + arrowWidth);
            DrawLine(setPoint, Width - borderSpace - arrowHeight, lineY - arrowWidth, Width - borderSpace, lineY);
            DrawLine(setPoint, Width - borderSpace - arrowHeight, lineY + arrowWidth, Width - borderSpace, lineY);
        }

        private static void DrawLine(Action<int, int> setPoint, int x1, int y1, int x2, int y2)
        {
            if (x1 == x2)
            {
                for (var y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    setPoint(x1, y);
                return;
            }
            
            var x = x1;
            do
            {
                var yFrom = (int)Math.Round((y2 - y1) * (x - x1) * 1.0 / (x2 - x1) + y1);
                var yTo = (int)Math.Round((y2 - y1) * ((x1 < x2 ? x + 1 : x - 1) - x1) * 1.0 / (x2 - x1) + y1);
                    
                var y = yFrom;
                do
                    setPoint(x, y);
                while (yFrom < yTo ? ++y < yTo : --y > yTo);
            }
            while (x1 < x2 ? ++x <= x2 : --x >= x2);
        }

        public Image GetImage(bool onlyClear = false)
        {
            using (var dBitmap = new DirectBitmap(Width, Height))
            {
                ClearOldPoints(dBitmap);

                if (onlyClear || Operation == null)
                    return dBitmap.Bitmap;

                DrawCoordinateSystem(dBitmap);

                //TODO
                int? last = null;
                for (var screenX = 0; screenX < Width; screenX++)
                {
                    int screenY;
                    if (GetPointAt(screenX, out screenY))
                    {
                        Draw(dBitmap, screenX, screenY);

                        if (last != null && Math.Abs(screenY - last.Value) > 1)
                        {
                            var df = GetDx(GetXFromScreenX(screenX - 1));
                            var dg = GetDx(GetXFromScreenX(screenX));

                            if (df > 0 && dg > 0 && last.Value > screenY ||
                                df < 0 && dg < 0 && last.Value < screenY)
                                for (var i = Math.Min(screenY, last.Value) + 1; i < Math.Max(screenY, last.Value); i++)
                                {
                                    Draw(dBitmap, screenX, i);
                                }

                        }
                        last = screenY;
                    }
                    else
                    {
                        if (last != null)
                        {
                            var df = GetDx(GetXFromScreenX(screenX - 1));
                            var dg = GetDx(GetXFromScreenX(screenX));

                            var asdf = 1.0;
                            var q = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1)));
                            var v = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf)));
                            var zz = 1e-10 * asdf;
                            var qwerty = 0;
                            while (asdf > 1e-10 && asdf <= 1 && v >= 0 && asdf - zz <= 1 && qwerty++ < 1000)
                            {
                                var z2 = GetDx(GetXFromScreenX(screenX - 1 + asdf - zz));
                                var v2 = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf - zz)));

                                if (v2 > q || z2 < 0)
                                {
                                    zz *= 2;
                                    continue;
                                }

                                //var z = GetDx(GetXFromScreenX(screenX - 1 + asdf - zz));
                                //v = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf - zz)));
                                if (v2 < 0)
                                    break;

                                if (v2 < q && z2 > 0)
                                {
                                    asdf -= zz;
                                    zz = 1e-10 * asdf;
                                    continue;
                                }

                                asdf /= 2;
                                //v = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf - zz)));

                            }


                            if (v < 0 && df > 0 && dg > 0)
                                for (int i = 0; i < last.Value; i++)
                                    Draw(dBitmap, screenX, i);
                        }

                        last = null;
                    }
                }
                return dBitmap.Bitmap;
            }
        }

        private double GetDx(double x)
        {
            var eps = x * 1e-11;
            if (x == 0)
                eps = double.Epsilon;

            while (GetYFromX(x) == GetYFromX(x + eps) && eps < 2)
                eps *= 2;

            if (eps >= 2)
                return 0;

            var value = (GetYFromX(x + eps) - GetYFromX(x)) / eps;
            return value;

            //return x >= 0 ? value : -value;
        }

        //private double GetDdx(double x)
        //{
        //    var eps = x * 1e-11;
        //    if (x == 0)
        //        eps = double.Epsilon;

        //    while (GetYFromX(x) == GetYFromX(x + eps) && eps < 2)
        //        eps *= 2;

        //    if (eps >= 2)
        //        return 0;

        //    double value = (GetYFromX(x + eps) - GetYFromX(x)) / eps;
        //    double value2 = GetDx(x + eps);
        //    return (value2 / value) / eps;

        //    //return x >= 0 ? value : -value;
        //}

        private DoublePoint Scale => new DoublePoint(Math.Pow(1.5, _scaleShift.X), Math.Pow(1.5, _scaleShift.Y));

        internal double GetXFromScreenX(double screenX) => screenX * Scale.X + _movement.X;
        internal int GetScreenXFromX(double x) => Convert.ToInt32((x - _movement.X) / Scale.X);

        internal double GetYFromX(double x) => Operation.Result(x);

        private int GetScreenYFromY(double y) => Convert.ToInt32(-(y + _movement.Y) / Scale.Y) + Height / 2;
        private double GetScreenYFromY2(double y) => -(y + _movement.Y) / Scale.Y + Height / 2;

        private bool GetPointAt(double screenX, out int value)
        {
            try
            {
                value = GetScreenYFromY(GetYFromX(GetXFromScreenX(screenX)));

                if (value >= 0 && value < Height)
                    return true;
            }
            catch { }

            value = 0;
            return false;
        }

        private class DoublePoint
        {
            public double X { get; set; }
            public double Y { get; set; }

            public DoublePoint() : this(0, 0) { }

            public DoublePoint(double x, double y)
            { X = x; Y = y; }

            public static implicit operator DoublePoint(System.Drawing.Point point)
                => new DoublePoint(point.X, point.Y);

            public static DoublePoint operator +(DoublePoint doublePoint, DoublePoint point2)
                => new DoublePoint(doublePoint.X + point2.X, doublePoint.Y + point2.Y);

            public static DoublePoint operator -(DoublePoint doublePoint, System.Drawing.Point point2)
                => new DoublePoint(doublePoint.X - point2.X, doublePoint.Y - point2.Y);

            public static DoublePoint operator -(DoublePoint doublePoint, double value)
                => new DoublePoint(doublePoint.X - value, doublePoint.Y - value);

            public static DoublePoint operator *(DoublePoint doublePoint, DoublePoint point2)
                => new DoublePoint(doublePoint.X * point2.X, doublePoint.Y * point2.Y);
        }
    }
}
