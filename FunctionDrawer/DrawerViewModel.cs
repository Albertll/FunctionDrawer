using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FunctionDrawer.Operations;
using FunctionDrawer.Properties;

namespace FunctionDrawer
{
    internal class DrawerViewModel : NotifyPropertyChanged, IDrawerViewModel
    {
        #region Fields

        private readonly FunctionCalculator _functionCalc = new FunctionCalculator();
        private readonly List<IOperation> _operations = new List<IOperation>();

        private string _errorMessage;
        private string _function;
        private int _width;
        private int _height;
        private DoublePoint _moveStartPoint;

        #endregion

        #region Properties

        public IEnumerable<IOperation> Operations
            => _operations;

        public IFunctionCalculator FunctionCalc
            => _functionCalc;

        public int Height
        {
            get { return _height; }
            set
            {
                if (value == 0) return;

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
                if (value == 0) return;

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
                _functionCalc.Movement = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Public methods

        public DrawerViewModel()
        {
            Function = "sin(x);x";
            //Function = "sin(x);cos(x);sin(-x);x/4;x^0,5";
        }

        public void ChangeScale(int locationX, int locationY, double scaleX, double scaleY)
        {
            ShortMove(locationX, locationY);

            FunctionCalc.ScaleFactor.X -= (float) scaleX;
            FunctionCalc.ScaleFactor.Y -= (float) scaleY;

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

            if (!Operations.Any())
                return;
            
            var task = Task.Run(() => CalcOperationPoints());

            DrawCoordinateSystem(graphics);

            var operationPoints = task.Result;

            var colors = new[] {Color.Black, Color.Red, Color.Chartreuse, Color.Coral, Color.Fuchsia, Color.Aqua, Color.Blue, Color.Yellow};
            var i = 0;

            foreach (var operation in Operations)
            {
                var points = operationPoints[operation];
                var pen = new Pen(colors[i++]);
                points.ForEach(p => graphics.DrawLines(pen, p));
            }
        }

        #endregion

        #region Private methods

        private void EvaluateOperation(string input)
        {
            _operations.Clear();
            try
            {
                var evaluator = new ExpressionEvaluator();

                _operations.AddRange(input
                    .Split(';')
                    .Select(expr => evaluator.Evaluate(expr)));
                
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(Resources.Error, ex.Message);
            }
        }

        private void ShortMove(int x, int y)
            => Movement += new DoublePoint(x, y) * FunctionCalc.Scale;

        private IDictionary<IOperation, IEnumerable<Point[]>> CalcOperationPoints()
        {
            var operationPoints = new ConcurrentDictionary<IOperation, IEnumerable<Point[]>>();

            Parallel.ForEach(Operations, operation =>
            {
                var points = GetFunctionPoints(operation);
                operationPoints.TryAdd(operation, points);
            });

            return operationPoints;
        }

        #region Coordinate system

        private void DrawCoordinateSystem(Graphics graphics)
        {
            const int borderSpace = 10;

            var pen = new Pen(Color.Blue);
            var brush = new SolidBrush(Color.Blue);
            var font = new Form().Font;

            // vertical line
            var lineX = MinMax(
                (int) FunctionCalc.GetScreenXFromX(0),
                borderSpace * 2,
                Width - borderSpace * 2);

            graphics.DrawLines(pen, GetLineWithArrow(lineX, borderSpace, lineX, Height - borderSpace));

            foreach (var y in GetLabelPoints(FunctionCalc.Scale.Y, FunctionCalc.GetYFromScreenY(Height - borderSpace),
                borderSpace, FunctionCalc.GetScreenYFromY))
            {
                graphics.DrawLine(pen,
                    lineX - 2, (int) y,
                    lineX + 2, (int) y);

                var value = (float) FunctionCalc.GetYFromScreenY(y);
                if (Math.Abs(value) > 1e-12)
                    graphics.DrawString(value.ToString(CultureInfo.InvariantCulture),
                        font, brush, lineX + 5, (int) y - 4);
            }

            // horizontal line
            var lineY = MinMax(
                (int) FunctionCalc.GetScreenYFromY(0),
                borderSpace * 2,
                Height - borderSpace * 2);

            graphics.DrawLines(pen, GetLineWithArrow(borderSpace, lineY, Width - borderSpace, lineY));

            foreach (var x in GetLabelPoints(FunctionCalc.Scale.X, FunctionCalc.GetXFromScreenX(borderSpace),
                Width - borderSpace, FunctionCalc.GetScreenXFromX))
            {
                graphics.DrawLine(pen,
                    (int) x, lineY - 2,
                    (int) x, lineY + 2);

                var value = (float) FunctionCalc.GetXFromScreenX(x);
                if (Math.Abs(value) > 1e-12)
                    graphics.DrawString(value.ToString(CultureInfo.InvariantCulture),
                        font, brush, (int) x - 4, lineY + 5);
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

        private static IEnumerable<double> GetLabelPoints(double scaleFactor, double start, double end,
            Func<double, double> getScreenVFromV)
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
        private IEnumerable<Point[]> GetFunctionPoints(IOperation operation)
        {
            var allPoints = new List<Point[]>();



            //aaa = FunctionCalc.Scale.X;
            //bbb= Movement.X;
            //aaaY = FunctionCalc.Scale.Y;
            //bbbY= Movement.Y;
            //TODO
            int? last = null;
            var points = new List<Point>();

            //s4.Start();
            //var rr = new ConcurrentDictionary<int, int?>();

            //var r = Enumerable.Range(0, Width);
            //Parallel.ForEach(r, screenX =>
            //{
            //    int screenY;
            //    var tt = TryGetPointAt(operation, screenX, out screenY);
            //    rr.TryAdd(screenX, tt ? (int?)screenY : null);
            //});
            //s4.Stop();
            
            for (var screenX = 0; screenX < Width; screenX++)
            {
                int screenY;
                var exists = TryGetPointAt(operation, screenX, out screenY);
                //var val = rr[screenX];
                //var tt = val.HasValue;
                //screenY = val.GetValueOrDefault();

                if (exists)
                {
                    if (last != null /*&& Math.Abs(screenY - last.Value) > 1*/)
                    {
                        //var x1 = MinMax(screenX - 1, -1, Width);
                        var y1 = MinMax(last.Value, -1, Height);
                        var x2 = MinMax(screenX, -1, Width);
                        var y2 = MinMax(screenY, -1, Height);

                        if (!(y2 == -1 && y1 == Height || y1 == -1 && y2 == Height))
                            //graphics.DrawLine(new Pen(color), x1, y1, x2, y2);
                        {
                            //if (y1 != y2)
                            //points.Add(new Point(x2, y1));
                            points.Add(new Point(x2, y2));
                        }

                    }
                }
                else
                {
                    if (points.Count > 1)
                        allPoints.Add(points.ToArray());

                    points.Clear();
                }

                last = screenY;
            }

            if (points.Count > 1)
                allPoints.Add(points.ToArray());

            return allPoints;
        }
        
        //Dictionary<double, double> zz = new Dictionary<double, double>();
        //private double aaa;
        //private double bbb;
        //private double aaaY;
        //private double bbbY;
        
        private bool TryGetPointAt(IOperation operation, double screenX, out int value)
        {
            value = -1;

            var x = FunctionCalc.GetXFromScreenX(screenX);

            var result = FunctionCalc.GetYFromX(operation, x);

            if (double.IsNaN(result) || double.IsInfinity(result))
                return false;

            result = FunctionCalc.GetScreenYFromY(result);

            //if (result > Height)
            //    result = Height;
            //if (result < -1)
            //    result = -1;
            result = MinMax(result, -1, Height);

            value = (int) result;
            //if (value >= 0 && value < Height)
            {
                return true;
            }

            //return false;
        }

        #endregion
    }
}
