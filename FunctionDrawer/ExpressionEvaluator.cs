using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FunctionDrawer
{
    public class ExpressionEvaluator
    {
        private Stack<string> _stack;
        private IList<string> _rnp;
        private StringBuilder _temp;
        private readonly string[] _funcs = { "+", "-", "*", "/", "^", "sin", "cos", "tan", "neg" };

        public IOperation Evaluate(string text)
        {
            MakeRnp(text);

            var operationStack = new Stack<Operation>();

            if (_rnp.Count == 0)
                throw new Exception("Zero arguments");
            
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

                if (operationStack.Count == 0)
                    throw new ArgumentException();

                var right = operationStack.Pop();
                switch (value)
                {
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

                //if (value.Equals("-") && (i - 2 < 0 || funcs.Contains(_rnp[i-2])))
                //{
                //    operationStack.Push(new Negative(right));
                //    continue;
                //}
                if (operationStack.Count == 0)
                    throw new IndexOutOfRangeException();

                Operation left = operationStack.Pop();
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
                }
            }

            if (operationStack.Count > 1)
                throw new Exception("Too many");
            return operationStack.Pop();
        }

        private void MakeRnp(string text)
        {
            text = Regex.Replace(text, @"\s+", ""); //spacje
            text = Regex.Replace(text, @"(?<=[^\d\)]|^)\-(\d+)", "neg($1)");
            text = Regex.Replace(text, @"(?<=\d)\(", "*(");
            text = Regex.Replace(text, @"\)(?=\d)", ")*");
            text = Regex.Replace(text, @"\w+\(\w+\)=(.*)", "$1");

            _stack = new Stack<string>();
            _temp = new StringBuilder();
            _rnp = new List<string>();

            for (int i = 0; i < text.Length; i++)
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
                        throw new NotEnoughBrackets();

                    _stack.Pop();
                }
            }

            while (_stack.Count != 0)
            {
                string op = _stack.Pop();

                if (op.Equals("("))
                    throw new NotEnoughBrackets();

                if (!_funcs.Contains(op))
                    throw new FormatException();

                _rnp.Add(op);
            }
        }

        public class NotEnoughBrackets : ArithmeticException { }

        private bool EvaluateNumber(char c, char next)
        {
            if (char.IsDigit(c) && (_temp.Length == 0 || IsNumber(_temp.ToString())))
                _temp.Append(c);

            if (IsNumber(_temp.ToString()) && (c.Equals('.') || c.Equals(',')))
                _temp.Append(c);

            if (!IsNumber(_temp.ToString()) && (char.IsDigit(next) || next.Equals('.') || next.Equals(',')))
                return false;

            if (char.IsDigit(next) || (IsNumber(_temp.ToString()) && next.Equals('.') || next.Equals(',')))
                return true;

            if ((_temp.Length != 0 && IsNumber(_temp.ToString())))
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
                string op = _temp.ToString();
                _temp.Clear();

                while (_stack.Count != 0 && !HasHigherPrior(op, _stack.Peek()))
                    _rnp.Add(_stack.Pop());

                _stack.Push(op);

                return true;
            }

            return false;
        }

        private bool HasHigherPrior(string op1, string op2) => GetPrior(op1) > GetPrior(op2);

        private int GetPrior(string op)
        {
            switch (op)
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
                    return 4;
                case "neg":
                    return 5;
                default:
                    return 0;
            }
        }

        private static bool IsNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            bool dot = false;

            foreach (char c in text)
                if (!char.IsDigit(c))
                {
                    if (!dot && (c.Equals('.') || c.Equals(',')))
                            dot = true;
                    else
                        return false;
                }

            return true;
        }
    }
}
