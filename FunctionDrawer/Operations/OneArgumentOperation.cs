namespace FunctionDrawer.Operations
{
    internal abstract class OneArgumentOperation : Operation
    {
        protected OneArgumentOperation(Operation right)
            : base(null, right) { }
    }
}