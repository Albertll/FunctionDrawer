﻿using FunctionDrawer.Operations;

namespace FunctionDrawer
{
    internal interface IFunctionCalculator
    {
        DoublePoint Movement { get; }
        DoublePoint Scale { get; }
        DoublePoint ScaleFactor { get; }

        double GetScreenXFromX(double x);
        double GetScreenYFromY(double y);
        double GetXFromScreenX(double screenX);
        double GetYFromScreenY(double screenY);
        double GetYFromX(IOperation operation, double x);
    }
}