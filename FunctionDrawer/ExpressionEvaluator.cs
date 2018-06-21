using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FunctionDrawer.Operations;
using FunctionDrawer.Properties;

namespace FunctionDrawer
{
    public class ExpressionEvaluator
    {
        private Stack<string> _stack;
        private IList<string> _rnp;
        private StringBuilder _temp;
        private readonly string[] _funcs = { "+", "-", "*", "/", "^", "sin", "cos", "tan", "neg", "abs", "log", "abs", "pi" };

        public IOperation Evaluate(string text)
        {
            MakeRnp(text.ToLowerInvariant());

            var operationStack = new Stack<Operation>();

            if (_rnp.Count == 0)
                throw new Exception(Resources.EmptyExpression);
            
            foreach (var value in _rnp)
            {
                if (IsNumber(value))
                {
                    operationStack.Push(new Value(double.Parse(value)));
                    continue;
                }

                if (value.Equals("x"))
                {
                    operationStack.Push(new X());
                    continue;
                }

                if (value.Equals("pi"))
                {
                    operationStack.Push(new Pi());
                    continue;
                }

                if (operationStack.Count == 0)
                    throw new Exception(Resources.LackOfFunctionArgument);

                var right = operationStack.Pop();
                switch (value)
                {
                    case "abs":
                        operationStack.Push(new Abs(right));
                        continue;
                    case "log":
                        operationStack.Push(new Log(right));
                        continue;
                    case "sin":
                        operationStack.Push(new Sinus(right));
                        continue;
                    case "cos":
                        operationStack.Push(new Cosinus(right));
                        continue;
                    case "tan":
                        operationStack.Push(new Tangens(right));
                        continue;
                    case "neg":
                        operationStack.Push(new Negative(right));
                        continue;
                }
                
                if (operationStack.Count == 0)
                    throw new Exception(Resources.IncompleteExpression);

                var left = operationStack.Pop();
                switch (value)
                {
                    case "+":
                        operationStack.Push(new Addition(left, right));
                        continue;
                    case "-":
                        operationStack.Push(new Subtraction(left, right));
                        continue;
                    case "*":
                        operationStack.Push(new Multiplication(left, right));
                        continue;
                    case "/":
                        operationStack.Push(new Division(left, right));
                        continue;
                    case "^":
                        operationStack.Push(new Power(left, right));
                        continue;
                    default:
                        throw new Exception(Resources.InvalidOperator);
                }
            }

            if (operationStack.Count > 1)
                throw new Exception(Resources.TooManyOperators);
            return operationStack.Pop();
        }

        private void MakeRnp(string text)
        {
            // remove spaces
            text = Regex.Replace(text, @"\s+", "");

            // change e.g. "f(t) = 4t" to "f(x) = 4x"
            if (Regex.IsMatch(text, @"\w+\((\w+)\)=(.*)"))
                text = Regex.Replace(text, Regex.Replace(text, @"\w+\((\w+)\)=(.*)", "$1"), "x");

            // change e.g. "f(x) = 4x" to "4x"
            text = Regex.Replace(text, @"\w+\(\w+\)=(.*)", "$1");

            // change expression with negative, e.g. "-sin(-x)" to "0-sin(0-x)"
            text = Regex.Replace(text, @"^-.*", "0$0");
            text = Regex.Replace(text, @"(?<=\()-.*?", "0$0");

            // add multiplication to bracket expression or x
            text = Regex.Replace(text, @"(?<=[\d\)x])(?=[\(x])", "*");
            text = Regex.Replace(text, @"\)(?=\d)", ")*");

            _stack = new Stack<string>();
            _temp = new StringBuilder();
            _rnp = new List<string>();

            for (var i = 0; i < text.Length; i++)
            {
                var current = text[i];
                var next = i + 1 < text.Length ? text[i + 1] : '\0';

                if (current.Equals('x'))
                {
                    _rnp.Add("x");
                    continue;
                }

                if (EvaluateNumber(current, next))
                    continue;

                if (EvaluateFunction(current, next))
                    continue;

                if (current.Equals('('))
                    _stack.Push("(");

                if (current.Equals(')'))
                {
                    while (_stack.Count != 0 && !_stack.Peek().Equals("("))
                        _rnp.Add(_stack.Pop());

                    if (_stack.Count == 0 || !_stack.Peek().Equals("("))
                        throw new Exception(Resources.NotEnoughBrackets);

                    _stack.Pop();
                }
            }

            while (_stack.Count != 0)
            {
                var op = _stack.Pop();

                if (op.Equals("("))
                    throw new Exception(Resources.NotEnoughBrackets);

                if (!_funcs.Contains(op))
                    throw new Exception(Resources.InvalidFunction);

                _rnp.Add(op);
            }
        }

        private bool EvaluateNumber(char c, char next)
        {
            if (char.IsDigit(c) && (_temp.Length == 0 || IsNumber(_temp.ToString())))
                _temp.Append(c);

            if (IsNumber(_temp.ToString()) && (c.Equals('.') || c.Equals(',') || c.Equals('e') || c.Equals('-')))
                _temp.Append(c);

            if (!IsNumber(_temp.ToString()) && (char.IsDigit(next) || next.Equals('.') || next.Equals(',')))
                return false;

            if (char.IsDigit(next) || IsNumber(_temp.ToString()) && next.Equals('.') || next.Equals(','))
                return true;

            if (next.Equals('e') && IsNumber(_temp.ToString()))
                return true;

            if (c.Equals('e') && next.Equals('-'))
                return true;

            if (_temp.Length != 0 && IsNumber(_temp.ToString()))
            {
                _rnp.Add(_temp.ToString());
                _temp.Clear();
            }

            return false;
        }
        
        private bool EvaluateFunction(char c, char next)
        {
            if (char.IsLetter(c) || _funcs.Contains(c.ToString()))
                _temp.Append(c);

            if (_funcs.Contains(_temp.ToString()) || !char.IsLetter(next) && _temp.Length != 0 && _funcs.Contains(_temp.ToString()))
            {
                var op = _temp.ToString();
                _temp.Clear();

                while (_stack.Count != 0 && !HasHigherPriority(op, _stack.Peek()))
                    _rnp.Add(_stack.Pop());

                _stack.Push(op);

                return true;
            }

            return false;
        }

        private static bool HasHigherPriority(string op1, string op2) => GetOperatorPriority(op1) > GetOperatorPriority(op2);

        private static int GetOperatorPriority(string @operator)
        {
            switch (@operator)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                    return 3;
                case "sin":
                case "cos":
                case "tan":
                case "log":
                case "abs":
                    return 4;
                case "neg":
                case "pi":
                    return 5;
                default:
                    return 0;
            }
        }

        private static bool IsNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var dot = false;
            var e = false;

            foreach (var c in text)
                if (!char.IsDigit(c))
                {
                    if (!dot && (c.Equals('.') || c.Equals(',')))
                        dot = true;
                    else
                    {
                        if (!e && c.Equals('e'))
                            e = true;
                        else if (!c.Equals('-') || !e)
                            return false;

                    }
                }

            return true;
        }
    }
}
