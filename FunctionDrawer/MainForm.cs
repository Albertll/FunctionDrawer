using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FunctionDrawer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            EvaluateOperation();

            _width = ClientSize.Width;
            _height = ClientSize.Height;

            Paint += (sender, e) => Redraw();
            Resize += OnResize;
        }

        IList<int> _currentPoints = new List<int>();
        IOperation _operation;
        int _height = 900;
        int _width = 1600;

        Point _scaleShift = new Point(-7, -7);
        Point _dPos = new Point();
        Point _mouseStart;
        Point _mousePos;

        private void EvaluateOperation()
        {
            IOperation operation = null;
            try
            {
                operation = new ExpressionEvaluator().Evaluate(functionInputTextBox.Text);
            }
            catch (DivideByZeroException)
            {
                yLabel.Text = "Division by 0";
            }
            catch (ExpressionEvaluator.NotEnoughBrackets)
            {
                yLabel.Text = "Not enough brackets";
            }
            catch (ArgumentException)
            {
                yLabel.Text = "Missing argument";
            }
            catch (Exception ex)
            {
                yLabel.Text = "Exception: " + ex.Message;
            }
            finally
            {
                _operation = operation;
            }
        }


        private void Redraw()
        {
            if (_operation == null)
                return;

            using (var bitmap = new DirectBitmap(_width, _height))
            {
                BeginDrawing(bitmap);

                int? last = null;
                //for (int screenX = -width / 2; screenX < width / 2; screenX++)
                for (int screenX = 0 / 2; screenX < _width; screenX++)
                {
                    int screenY;
                    if (GetPointAt(screenX, out screenY))
                    {
                        Draw(bitmap, screenX, screenY);

                        if (last != null && Math.Abs(screenY - last.Value) > 1)
                        {
                            //int? ll = GetPointAt(i-0.5);
                            //if (ll != null)
                            //{
                            //    bitmap.Bits[i + width * ll.Value] = Color.Black.ToArgb();// int.MaxValue / 4*3 + r.Next() / 4;
                            //    currentPoints.Add(i + width * ll.Value);
                            //}

                            //double fa = GetYFromX(GetXFromScreenX(screenX - 1) * 0.9999999);
                            //double fb = GetYFromX(GetXFromScreenX(screenX - 1));
                            //double fc = GetYFromX(GetXFromScreenX(screenX - 1) * 1.0000001);
                            //double dfa = GetXFromScreenX(screenX - 1) >= 0 ? fb - fa : fb - fc;
                            //double dfb = fc - fb;
                            //double ddf = dfb - dfa;

                            //double ga = GetYFromX(GetXFromScreenX(screenX) * 0.9999999);
                            //double gb = GetYFromX(GetXFromScreenX(screenX));
                            //double gc = GetYFromX(GetXFromScreenX(screenX) * 1.0000001);
                            //double dga = GetXFromScreenX(screenX) >= 0 ? gb - ga : gb - gc;
                            //double dgb = gc - gb;
                            //double ddg = dgb - dga;
                            double df = GetDx(GetXFromScreenX(screenX - 1));
                            double dg = GetDx(GetXFromScreenX(screenX));
                            //if (GetXFromScreenX(screenX - 1) < 0) df = -df;
                            //if (GetXFromScreenX(screenX) < 0) dg = -dg;

                            //if (dfa * dga > 0 && ddf * ddg > 0)
                            if (df > 0 && dg > 0 && last.Value > screenY ||
                                df < 0 && dg < 0 && last.Value < screenY)
                                for (int i = Math.Min(screenY, last.Value) + 1; i < Math.Max(screenY, last.Value); i++)
                                {
                                    Draw(bitmap, screenX, i);
                                }

                        }
                        last = screenY;
                    }
                    else
                    {
                        if (last != null)
                        {
                            double df = GetDx(GetXFromScreenX(screenX - 1));
                            double dg = GetDx(GetXFromScreenX(screenX));
                            //if (GetXFromScreenX(screenX - 1) < 0) df = -df;
                            //if (GetXFromScreenX(screenX) < 0) dg = -dg;

                            //double asdf = 1;
                            //int v = GetScreenYFromY(GetYFromX(GetXFromScreenX(screenX - 1 + asdf)));
                            //int w = GetScreenYFromY(GetYFromX(GetXFromScreenX(screenX - asdf)));
                            //while (asdf > 0 && v >= 0)
                            //{
                            //    asdf /= 2;
                            //    v = GetScreenYFromY(GetYFromX(GetXFromScreenX(screenX - 1 + asdf)));
                            //    w = GetScreenYFromY(GetYFromX(GetXFromScreenX(screenX - asdf)));

                            //}
                            //if (w < 0) v = w;

                            double asdf = 1;
                            double q = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1)));
                            double v = GetScreenYFromY2(GetYFromX(GetXFromScreenX(screenX - 1 + asdf)));
                            double zz = 1e-10 * asdf;
                            int qwerty = 0;
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
                                    Draw(bitmap, screenX, i);
                        }

                        last = null;
                    }
                }

                EndDrawing(bitmap);
            }
        }

        private double GetDx(double x)
        {
            double eps = Double.Epsilon;
            eps = x * 1e-11;
            if (x == 0)
                eps = Double.Epsilon;

            while (GetYFromX(x) == GetYFromX(x + eps) && eps < 2)
                eps *= 2;

            if (eps >= 2)
                return 0;

            double value = (GetYFromX(x + eps) - GetYFromX(x)) / eps;
            return value;

            //return x >= 0 ? value : -value;
        }

        private double GetDdx(double x)
        {
            double eps = Double.Epsilon;
            eps = x * 1e-11;
            if (x == 0)
                eps = Double.Epsilon;

            while (GetYFromX(x) == GetYFromX(x + eps) && eps < 2)
                eps *= 2;

            if (eps >= 2)
                return 0;

            double value = (GetYFromX(x + eps) - GetYFromX(x)) / eps;
            double value2 = GetDx(x + eps);
            return (value2 / value) / eps;

            //return x >= 0 ? value : -value;
        }

        private void Draw(DirectBitmap bitmap, int x, int y)
        {
            int pixel = x + _width * y;
            bitmap.Bits[pixel] = Color.Black.ToArgb();// int.MaxValue / 4*3 + r.Next() / 4;
            _currentPoints.Add(pixel);
        }

        private void BeginDrawing(DirectBitmap bitmap)
        {
            var oldPoints = _currentPoints.ToList();
            _currentPoints.Clear();

            foreach (int i in oldPoints)
                bitmap.Bits[i] = DefaultBackColor.ToArgb();
        }

        private void EndDrawing(DirectBitmap bitmap)
        {
            //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Graphics graphics = CreateGraphics();
            graphics.DrawImage(bitmap.Bitmap, 0, 0, _width, _height);
        }

        private new Point Scale => new Point(Math.Pow(1.5, _scaleShift.X), Math.Pow(1.5, _scaleShift.Y));

        private double GetXFromScreenX(double screenX) => screenX * Scale.X + _dPos.X;

        private double GetYFromX(double x) => _operation.Result(x);

        private int GetScreenYFromY(double y) => Convert.ToInt32(-(y + _dPos.Y) / Scale.Y) + _height / 2;
        private double GetScreenYFromY2(double y) => -(y + _dPos.Y) / Scale.Y + _height / 2;

        private bool GetPointAt(double screenX, out int value)
        {
            try
            {
                value = GetScreenYFromY(GetYFromX(GetXFromScreenX(screenX/* - width / 2*/)));

                if (value >= 0 && value < _height)
                    return true;
            }
            catch { }

            value = 0;
            return false;
        }

        #region Events

        private void OnResize(object sender, EventArgs e)
        {
            if (ClientSize.Width == 0 || ClientSize.Height == 0)
                return;

            using (var bitmap = new DirectBitmap(_width, _height))
            {
                BeginDrawing(bitmap);
                EndDrawing(bitmap);
            }

            _width = ClientSize.Width;
            _height = ClientSize.Height;

            Redraw();
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int dir = e.Delta / Math.Abs(e.Delta);

            _dPos.X = GetXFromScreenX(e.X);

            _scaleShift -= dir;

            //_dPos.X = (_dPos.X - (e.X - 100) * Scale.X) * 1.5 * dir + (e.X - 100) * Scale.X;
            // _dPos.Y = Convert.ToInt32((_dPos.Y - (e.Y - 8 - 350)) * 1.5 * dir + (e.Y - 8 - 350));

            if (e.Button == MouseButtons.Left)
                OnMouseDown(sender, e);

            Redraw();
            OnMouseMove(sender, e);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _mouseStart = _dPos;
            _mousePos = e.Location;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _mouseStart != null)
            {
                _dPos = _mouseStart + (_mousePos - e.Location) * Scale;
                Redraw();
            }
            else
            {
                double x = GetXFromScreenX(e.X);
                xLabel.Text = $"X = {x.ToString()}";
                try
                {
                    yLabel.Text = $"Y = {GetYFromX(x)}";
                    //yLabel.Text = $"DY = {operation.D(x)}";
                }
                catch
                {
                    if (_operation != null)
                        yLabel.Text = $"Y = NaN";
                }
            }
        }

        private void functionInputTextBox_TextChanged(object sender, EventArgs e)
        {
            EvaluateOperation();
            Redraw();
        }

        #endregion
    }

    internal class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point() : this(0, 0) { }

        public Point(double x, double y)
        { X = x; Y = y; }

        public static implicit operator Point(System.Drawing.Point point)
            => new Point(point.X, point.Y);

        public static Point operator +(Point point, Point point2)
            => new Point(point.X + point2.X, point.Y + point2.Y);

        public static Point operator -(Point point, System.Drawing.Point point2)
            => new Point(point.X - point2.X, point.Y - point2.Y);

        public static Point operator -(Point point, double value)
            => new Point(point.X - value, point.Y - value);

        public static Point operator *(Point point, Point point2)
            => new Point(point.X * point2.X, point.Y * point2.Y);
    }

    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
