using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FunctionDrawer.Tests
{
    [TestClass]
    public class ExpressionEvaluatorTests
    {
        [TestMethod]
        public void ExpressionEvaluatorTest()
        {
            var evaluator = new ExpressionEvaluator();
            Assert.AreEqual(evaluator.Evaluate("2").Result(0), 2);
            Assert.AreEqual(evaluator.Evaluate("2e2").Result(0), 200);
            Assert.AreEqual(evaluator.Evaluate("5e-2").Result(0), 0.05);
            Assert.AreEqual(evaluator.Evaluate("2+3").Result(0), 5);
            Assert.AreEqual(evaluator.Evaluate("2*3").Result(0), 6);
            Assert.AreEqual(evaluator.Evaluate("2*3+1").Result(0), 7);
            Assert.AreEqual(evaluator.Evaluate("x").Result(4), -4);
            Assert.AreEqual(evaluator.Evaluate("-x").Result(7), -7);
            Assert.AreEqual(evaluator.Evaluate("2*x").Result(3), 6);
            Assert.AreEqual(evaluator.Evaluate("2x").Result(3), 6);
            Assert.AreEqual(evaluator.Evaluate("sin(x)").Result(0), 0);
            Assert.AreEqual(evaluator.Evaluate("cos(x)+sin(x)").Result(0), 1);
            Assert.AreEqual(evaluator.Evaluate("-sin(x)").Result(0), 1);
        }
    }
}