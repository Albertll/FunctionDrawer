using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunctionDrawer
{
    public partial class DrawerMainView : Form
    {
        public DrawerMainView()
        {
            InitializeComponent();

            _bsViewModel.DataSource = DrawerViewModel;

            DrawerViewModel.PropertyChanged += (s, e) => Redraw();
        }

        private IDrawerViewModel DrawerViewModel { get; } = new DrawerViewModel();

        private Point _mousePos;

        private void Redraw() => Invalidate();

        #region Override methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawerViewModel.Paint(e.Graphics);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Redraw();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            DrawerViewModel.Width = ClientSize.Width;
            DrawerViewModel.Height = ClientSize.Height;
            
            Redraw();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

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
                        BeginInvoke((Action)(() =>
                        {
                            DrawerViewModel.ChangeScale(e.Location.X, e.Location.Y, dirX, dirY);
                            _lbS.Text = $@"Scale: X = {DrawerViewModel.FunctionCalc.ScaleFactor.X}, Y = {DrawerViewModel.FunctionCalc.ScaleFactor.Y}";
                        }));
                        Thread.Sleep(30);
                    }
                }).Start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _mousePos = e.Location;
            DrawerViewModel.StartMoving();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DrawerViewModel.FunctionCalc.Operation == null)
            {
                _lbX.Text = _lbY.Text = string.Empty;
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                DrawerViewModel.MoveScreen(_mousePos.X - e.Location.X, _mousePos.Y - e.Location.Y);
            }
            else
            {
                var x = DrawerViewModel.FunctionCalc.GetXFromScreenX(e.X);
                _lbX.Text = $@"X = {x}";
                try
                {
                    _lbY.Text = $@"Y = {DrawerViewModel.FunctionCalc.GetYFromX(x)}";
                }
                catch
                {
                    if (DrawerViewModel.FunctionCalc.Operation != null)
                        _lbY.Text = $@"Y = NaN";
                }
            }
        }

        #endregion
    }
}
