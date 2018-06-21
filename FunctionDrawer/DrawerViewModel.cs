using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FunctionDrawer.Properties;

namespace FunctionDrawer
{
    internal class DrawerViewModel : NotifyPropertyChanged, IDrawerViewModel
    {
        #region Fields
        
        private string _errorMessage;
        private string _function;
        private int _width;
        private int _height;
        private DoublePoint _moveStartPoint;

        #endregion

        #region Properties
        
        private FunctionCalculator FunctionCalc { get; } = new FunctionCalculator();
        IFunctionCalculator IDrawerViewModel.FunctionCalc => FunctionCalc;

        public int Height
        {
            get { return _height; }
            set
            {
                var org = _height;
                _height = value;
                _moveStartPoint = null;
                ShortMove(0, (org - value) / 2);
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                var org = _width;
                _width = value;
                _moveStartPoint = null;
                ShortMove((org - value) / 2, 0);
            }
        }

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

        private DoublePoint Movement
        {
            get { return FunctionCalc.Movement; }
            set
            {
                FunctionCalc.Movement = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Public methods

        public DrawerViewModel()
        {
            //Function = "x";
            Function = "sin(x)";
        }

        public void ChangeScale(int locationX, int locationY, double scaleX, double scaleY)
        {
            ShortMove(locationX, locationY);

            FunctionCalc.ScaleFactor.X -= (float)scaleX;
            FunctionCalc.ScaleFactor.Y -= (float)scaleY;

            FunctionCalc.ScaleFactor.X = Math.Round(FunctionCalc.ScaleFactor.X * 100) / 100;
            FunctionCalc.ScaleFactor.Y = Math.Round(FunctionCalc.ScaleFactor.Y * 100) / 100;

            ShortMove(-locationX, -locationY);
        }

        public void StartMoving()
        {
            _moveStartPoint = Movement;
        }

        public void MoveScreen(int x, int y)
        {
            if (_moveStartPoint == null)
                return;

            Movement = _moveStartPoint + new DoublePoint(x, y) * FunctionCalc.Scale;
        }

        public void Paint(Graphics graphics)
        {
            //graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (FunctionCalc.Operation == null)
                return;

            DrawCoordinateSystem(graphics);
            
            DrawFunction(graphics);
        }

        #endregion

        #region Private methods

        private void EvaluateOperation(string input)
        {
            FunctionCalc.Operation = null;
            try
            {
                FunctionCalc.Operation = new ExpressionEvaluator().Evaluate(input);
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(Resources.Error, ex.Message);
            }
        }

        private void ShortMove(int x, int y)
            => Movement += new DoublePoint(x, y) * FunctionCalc.Scale;

        #region Coordinate system

        private void DrawCoordinateSystem(Graphics graphics)
        {
            const int borderSpace = 10;

            var pen = new Pen(Color.Blue);
            var brush = new SolidBrush(Color.Blue);
            var font = new Form().Font;

            // vertical line
            var lineX = MinMax(
                (int)FunctionCalc.GetScreenXFromX(0), 
                borderSpace * 2, 
                Width - borderSpace * 2);

            graphics.DrawLines(pen, GetLineWithArrow(lineX, borderSpace, lineX, Height - borderSpace));

            foreach (var y in GetLabelPoints(FunctionCalc.Scale.Y, FunctionCalc.GetYFromScreenY(Height - borderSpace), borderSpace, FunctionCalc.GetScreenYFromY))
            {
                graphics.DrawLine(pen, 
                    lineX - 2, (int)y, 
                    lineX + 2, (int)y);

                var value = (float)FunctionCalc.GetYFromScreenY(y);
                if (Math.Abs(value) > 1e-12)
                    graphics.DrawString(value.ToString(CultureInfo.InvariantCulture),
                        font, brush, lineX + 5, (int)y - 4);
            }

            // horizontal line
            var lineY = MinMax(
                (int)FunctionCalc.GetScreenYFromY(0), 
                borderSpace * 2, 
                Height - borderSpace * 2);

            graphics.DrawLines(pen, GetLineWithArrow(borderSpace, lineY, Width - borderSpace, lineY));

            foreach (var x in GetLabelPoints(FunctionCalc.Scale.X, FunctionCalc.GetXFromScreenX(borderSpace), Width - borderSpace, FunctionCalc.GetScreenXFromX))
            {
                graphics.DrawLine(pen, 
                    (int)x, lineY - 2, 
                    (int)x, lineY + 2);

                var value = (float)FunctionCalc.GetXFromScreenX(x);
                if (Math.Abs(value) > 1e-12)
                    graphics.DrawString(value.ToString(CultureInfo.InvariantCulture),
                        font, brush, (int)x - 4, lineY + 5);
                else
                {
                    if (Math.Abs(value) > 0)
                    {

                    }
                }
            }
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        public static T MinMax<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;

            return value;
        }

        private static IEnumerable<double> GetLabelPoints(double scaleFactor, double start, double end, Func<double, double> getScreenVFromV)
        {
            // magic
            var a = Math.Round(-1.5 - Math.Log10(scaleFactor * 0.5) * 2) / 2;
            var unit = Math.Pow(10, 1 - Math.Floor(-0.5 + a)) / (2 - 2 * Math.Abs(a - Math.Floor(a)));

            var increasing = getScreenVFromV(start) < getScreenVFromV(end);
            var value = getScreenVFromV(start - start % unit);
            var current = start;

            while (increasing ? value < end : value > end)
            {
                yield return value;

                current += unit * 0.99999999;
                var newValue = getScreenVFromV(current - current % unit);
                if (Math.Abs(newValue - value) < 1e-14 && Math.Abs(current - current % unit) > 1e-14)
                    break;
                value = newValue;
            }
        }

        private static Point[] GetLineWithArrow(int x1, int y1, int x2, int y2)
        {
            const int arrowWidth = 5;
            const int arrowHeight = 10;

            if (x1 == x2)
                return new[]
                {
                    new Point(x2, y2),
                    new Point(x1, y1 + arrowHeight),
                    new Point(x1 - arrowWidth, y1 + arrowHeight),
                    new Point(x1, y1),
                    new Point(x1 + arrowWidth, y1 + arrowHeight),
                    new Point(x1, y1 + arrowHeight),
                };

            if (y1 == y2)
                return new[]
                {
                    new Point(x1, y1),
                    new Point(x2 - arrowHeight, y2),
                    new Point(x2 - arrowHeight, y2 - arrowWidth),
                    new Point(x2, y2),
                    new Point(x2 - arrowHeight, y2 + arrowWidth),
                    new Point(x2 - arrowHeight, y2),
                };
            return new Point[0];
        }

        #endregion

        private void DrawFunction(Graphics graphics)
        {
            //TODO
            int? last = null;
            for (var screenX = 0; screenX < Width; screenX++)
            {
                int screenY;
                if (TryGetPointAt(screenX, out screenY))
                {
                    //Draw(graphics, screenX, screenY);

                    // if (last != null /*&& Math.Abs(screenY - last.Value) > 1*/)
                    {
                        //var df = GetDx(GetXFromScreenX(screenX - 1));
                        //var dg = GetDx(GetXFromScreenX(screenX));

                        //if (df > 0 && dg > 0 && last.Value > screenY ||
                        //    df < 0 && dg < 0 && last.Value < screenY)
                        //    for (var i = Math.Min(screenY, last.Value) + 1; i < Math.Max(screenY, last.Value); i++)
                        //    {
                        //        //Draw(graphics, screenX, i);
                        //    }
                        //graphics.DrawLine(new Pen(Color.Black), screenX, screenY, screenX-1, last.Value);

                    }
                    if (last != null /*&& Math.Abs(screenY - last.Value) > 1*/)
                    {
                        var x1 = Math.Max(-1, Math.Min(Width, screenX));
                        var x2 = Math.Max(-1, Math.Min(Width, screenX - 1));
                        var y1 = Math.Max(-1, Math.Min(Height, screenY));
                        var y2 = Math.Max(-1, Math.Min(Height, last.Value));
                        //if (y1 != -1 && y2 != Height && y2 != -1 && y1 != Height)
                        if (!(y1 == -1 && y2 == Height || y2 == -1 && y1 == Height))
                            graphics.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                    }
                }
                last = screenY;
                //else
                //{
                //    if (last != null)
                //    {
                //        var df = GetDx(GetXFromScreenX(screenX - 1));
                //        var dg = GetDx(GetXFromScreenX(screenX));

                //        var asdf = 1.0;
                //        var q = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1)));
                //        var v = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf)));
                //        var zz = 1e-10 * asdf;
                //        var qwerty = 0;
                //        while (asdf > 1e-10 && asdf <= 1 && v >= 0 && asdf - zz <= 1 && qwerty++ < 1000)
                //        {
                //            var z2 = GetDx(GetXFromScreenX(screenX - 1 + asdf - zz));
                //            var v2 = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf - zz)));

                //            if (v2 > q || z2 < 0)
                //            {
                //                zz *= 2;
                //                continue;
                //            }

                //            //var z = GetDx(GetXFromScreenX(screenX - 1 + asdf - zz));
                //            //v = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf - zz)));
                //            if (v2 < 0)
                //                break;

                //            if (v2 < q && z2 > 0)
                //            {
                //                asdf -= zz;
                //                zz = 1e-10 * asdf;
                //                continue;
                //            }

                //            asdf /= 2;
                //            //v = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf - zz)));

                //        }

                //        //???
                //        //if (v < 0 && df > 0 && dg > 0)
                //        //    for (int i = 0; i < last.Value; i++)
                //        //        Draw(graphics, screenX, i);
                //    }

                //    last = null;
                //}
            }
        }

        //private double GetDx(double x)
        //{
        //    var eps = x * 1e-11;
        //    if (x == 0)
        //        eps = double.Epsilon;

        //    while (GetYFromX(x) == GetYFromX(x + eps) && eps < 2)
        //        eps *= 2;

        //    if (eps >= 2)
        //        return 0;

        //    var value = (GetYFromX(x + eps) - GetYFromX(x)) / eps;
        //    return value;

        //    //return x >= 0 ? value : -value;
        //}

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

        private bool TryGetPointAt(double screenX, out int value)
        {
            value = -1;
            try
            {
                var result = FunctionCalc.GetYFromX(FunctionCalc.GetXFromScreenX(screenX));
                if (double.IsNaN(result) || double.IsInfinity(result))
                    return false;

                result = FunctionCalc.GetScreenYFromY(result);

                if (result > Height)
                    result = Height;
                if (result < -1)
                    result = -1;

                value = (int)result;
                //if (value >= 0 && value < Height)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            //return false;
        }

        #endregion
    }
}
