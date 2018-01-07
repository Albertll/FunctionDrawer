using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace FunctionDrawer
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
            _bsViewModel.DataSource = ViewModel;

            ViewModel.Width = ClientSize.Width;
            ViewModel.Height = ClientSize.Height;
            ViewModel.BackgroundColor = DefaultBackColor;

            ViewModel.PropertyChanged += (s, e) => Redraw();
            Paint += (sender, e) => Redraw();
            Resize += OnResize;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Redraw();
        }

        private ViewModel ViewModel { get; } = new ViewModel();


        private Point _mousePos;

        private void Redraw()
        {
            var t = new Stopwatch();
            t.Start();
            using (var bitmap = ViewModel.GetImage())
                DrawImage(bitmap);
            t.Stop();
            //Console.WriteLine(t.Elapsed);
        }

        private void DrawImage(Image bitmap)
        {
            var graphics = CreateGraphics();
            //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.DrawImage(bitmap, 0, 0, ViewModel.Width, ViewModel.Height);
        }

        #region Events

        private void OnResize(object sender, EventArgs e)
        {
            if (ClientSize.Width == 0 || ClientSize.Height == 0)
                return;

            // clear screen
            using (var bitmap = ViewModel.GetImage(true))
                DrawImage(bitmap);
            
            ViewModel.Width = ClientSize.Width;
            ViewModel.Height = ClientSize.Height;

            Redraw();
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            //TODO
            //int dir = e.Delta / Math.Abs(e.Delta);

            //ViewModel.Movement.X = ViewModel.GetXFromScreenX(e.X);
            //ViewModel.Movement.Y = ViewModel.GetXFromScreenX(e.Y);

            //ViewModel.ScaleShift -= dir;
            //_lbY.Text = $"ScaleShift = {ViewModel.ScaleShift.X}";

            //ViewModel.Movement.X = (ViewModel.Movement.X - (e.X - 100) * ViewModel.Scale.X) * 1.5 * dir + (e.X - 100) * ViewModel.Scale.X;
            //ViewModel.Movement.Y = Convert.ToInt32((ViewModel.Movement.Y - (e.Y - 8 - 350)) * 1.5 * dir + (e.Y - 8 - 350));

            //if (e.Button == MouseButtons.Left)
            //    OnMouseDown(sender, e);

            //Redraw();
            //OnMouseMove(sender, e);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _mousePos = e.Location;
            ViewModel.StartMoving();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel.Operation == null)
            {
                _lbX.Text = _lbY.Text = string.Empty;
                return;
            }

            if (e.Button == MouseButtons.Left && _mousePos != null)
            {
                ViewModel.MoveScreen(_mousePos.X - e.Location.X, _mousePos.Y - e.Location.Y);
            }
            else
            {
                var x = ViewModel.GetXFromScreenX(e.X);
                _lbX.Text = $"X = {x}";
                try
                {
                    _lbY.Text = $"Y = {ViewModel.GetYFromX(x)}";
                    //yLabel.Text = $"DY = {ViewModel.Operation.D(x)}";
                }
                catch
                {
                    if (ViewModel.Operation != null)
                        _lbY.Text = $"Y = NaN";
                }
            }
        }

        #endregion
    }
}
