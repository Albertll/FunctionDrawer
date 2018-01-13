using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunctionDrawer
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
            _bsViewModel.DataSource = ViewModel;

            //ViewModel.Width = ClientSize.Width;
            //ViewModel.Height = ClientSize.Height;

            ViewModel.PropertyChanged += (s, e) => Redraw();
            DoubleBuffered = true;
        }

        private ViewModel ViewModel { get; } = new ViewModel();

        private Point _mousePos;

        private void Redraw() => Invalidate();

        #region Events


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ViewModel.Paint(e.Graphics);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Redraw();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ViewModel.Width = ClientSize.Width;
            ViewModel.Height = ClientSize.Height;
            
            Redraw();
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            var dirX = e.Delta / 5.0 / Math.Abs(e.Delta);
            var dirY = e.Delta / 5.0 / Math.Abs(e.Delta);
            if (ModifierKeys.HasFlag(Keys.Shift))
                dirX = 0;
            if (ModifierKeys.HasFlag(Keys.Control))
                dirY = 0;

            new Task(
                () =>
                {
                    for (var i = 0; i < 5; i++)
                    {
                        BeginInvoke((Action) (() =>
                        {
                            ViewModel.ChangeScale(e.Location.X, e.Location.Y, dirX, dirY);
                            _lbS.Text = $@"Scale: X = {ViewModel.ScaleFactor.X}, Y = {ViewModel.ScaleFactor.Y}";
                        }));
                        Thread.Sleep(30);
                    }
                }).Start();
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
