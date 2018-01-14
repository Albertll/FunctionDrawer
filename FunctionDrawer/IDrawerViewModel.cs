using System.ComponentModel;
using System.Drawing;

namespace FunctionDrawer
{
    internal interface IDrawerViewModel : INotifyPropertyChanged
    {
        string ErrorMessage { get; }
        string Function { get; }
        IFunctionCalculator FunctionCalc { get; }

        int Width { get; set; }
        int Height { get; set; }

        void Paint(Graphics graphics);
        void ChangeScale(int locationX, int locationY, double scaleX, double scaleY);
        void StartMoving();
        void MoveScreen(int x, int y);
    }
}