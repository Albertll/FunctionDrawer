namespace FunctionDrawer.Operations
{
    internal abstract class TwoArgumentsOperation : Operation
    {
        protected TwoArgumentsOperation(Operation left, Operation right)
            : base(left, right) { }
    }
}