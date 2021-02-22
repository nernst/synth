using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

using UnaryOp = System.Func<double, double>;
using BinaryOp = System.Func<double, double, double>;

namespace ErnstTech.SoundCore.Synthesis
{
    public class ParseError : Exception
    {
        public ParseError() : base()
        { }

        public ParseError(string message): base(message)
        { }
    }
    internal class ThrowingErrorListener<TSymbol> : IAntlrErrorListener<TSymbol>
    {
        #region IAntlrErrorListener<TSymbol> Implementation
        public void SyntaxError(TextWriter output, IRecognizer recognizer, TSymbol offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ParseError($"line {line}:{charPositionInLine} {msg}");
        }
        #endregion
    }
    public class ExpressionParser
    {
        internal class SynthesizerGrammarVisitor : SynthExpressionGrammarBaseVisitor<Func<double, double>>
        {
            IReadOnlyDictionary<string, double> _Constants = new Dictionary<string, double>()
            {
                { "e", Math.E },
                { "pi", Math.PI },
            };

            IReadOnlyDictionary<string, UnaryOp> _FuncMap = new Dictionary<string, UnaryOp>()
            {
                { "sin", Math.Sin },
                { "cos", Math.Cos },
                { "tan", Math.Tan },
                { "asin", Math.Asin },
                { "acos", Math.Acos },
                { "atan", Math.Atan },
                { "sqrt", Math.Sqrt },
                { "log", Math.Log10 },
                { "ln", Math.Log },
                { "abs", Math.Abs },
                { "saw", Waves.Saw },
                { "tri", Waves.Tri },
            };

            IReadOnlyDictionary<string, BinaryOp> _BinaryOps = new Dictionary<string, BinaryOp>()
            {
                { "+", (double l, double r) => l + r },
                { "-", (double l, double r) => l - r },
                { "*", (double l, double r) => l * r },
                { "/", (double l, double r) => l / r },
            };

            UnaryOp ComposeUnaryFunc(string unaryOp, UnaryOp arg)
            {
                if (string.IsNullOrWhiteSpace(unaryOp))
                    throw new ArgumentNullException(nameof(unaryOp));
                if (arg == null)
                    throw new ArgumentNullException(nameof(arg));

                if (_FuncMap.TryGetValue(unaryOp, out var op))
                    return (double t) => op(arg(t));

                if (unaryOp == "smooth")
                    return Smoother.Wrap(8, arg);

                throw new ArgumentException($"Unknown unary function: '{arg}'.");
            }

            UnaryOp ComposeBinaryFunc(string binaryOp, UnaryOp left, UnaryOp right)
            {
                if (string.IsNullOrWhiteSpace(binaryOp))
                    throw new ArgumentNullException(nameof(binaryOp));
                if (left == null)
                    throw new ArgumentNullException(nameof(left));
                if (right == null)
                    throw new ArgumentNullException(nameof(right));

                if (_BinaryOps.TryGetValue(binaryOp, out var op))
                    return (double t) => op(left(t), right(t));

                throw new ArgumentException($"Unknown binary operation: '{binaryOp}'");
            }

            public override Func<double, double> VisitEquation([NotNull] SynthExpressionGrammarParser.EquationContext context)
            {
                return Visit(context.expression(0));
            }

            UnaryOp ComposeChildren(IList<IParseTree> children)
            {
                if (children == null || children.Count <= 0)
                    throw new InvalidOperationException($"Expected at least one child. Received {children?.Count ?? 0}.");
                
                var res = Visit(children[0]);

                for (int i = 1; i < children.Count; i += 2)
                {
                    var op = children[i].GetText();
//                    var childText = children[i + 1].GetText();
                    var arg = Visit(children[i + 1]);
                    res = ComposeBinaryFunc(op, res, arg);
                }

                return res;
            }

            public override Func<double, double> VisitExpression([NotNull] SynthExpressionGrammarParser.ExpressionContext context) => ComposeChildren(context.children);
            public override UnaryOp VisitMultiplyingExpression([NotNull] SynthExpressionGrammarParser.MultiplyingExpressionContext context) => ComposeChildren(context.children);

            public override Func<double, double> VisitFunc([NotNull] SynthExpressionGrammarParser.FuncContext context)
            {
                var funcname = context.funcname().GetText();
                var arg = Visit(context.expression());

                if (_FuncMap.TryGetValue(funcname, out var func))
                    return (double t) => func(arg(t));
                else if (funcname == "adsr")
                    return new ADSREnvelope().Adapt(arg);
                else if (funcname == "smooth")
                    return Smoother.Wrap(32, arg);

                Debug.Assert(false);
                return base.VisitFunc(context);
            }

            public override UnaryOp VisitAtom([NotNull] SynthExpressionGrammarParser.AtomContext context)
            {
                if (context.children.Count == 3 && context.children[0].GetText() == "(" && context.children[2].GetText() == ")")
                    return Visit(context.children[1]);

                return base.VisitAtom(context);
            }

            public override Func<double, double> VisitScientific([NotNull] SynthExpressionGrammarParser.ScientificContext context)
            {
                if (double.TryParse(context.GetText(), out var value))
                    return (double _) => value;

                throw new InvalidOperationException($"Invalid literal: '{context.GetText()}'.");
            }

            public override UnaryOp VisitRandom([NotNull] SynthExpressionGrammarParser.RandomContext context)
            {
#if DEBUG
                var text = context.GetText();
                Debug.Assert(text == "rand");
#endif

                var random = new Random();

                return (double _) => 2.0 * (random.NextDouble() - 0.5);
            }

            public override Func<double, double> VisitConstant([NotNull] SynthExpressionGrammarParser.ConstantContext context)
            {
                if (_Constants.TryGetValue(context.GetText(), out var value))
                    return (double _) => value;

                throw new InvalidOperationException($"Invalid constant: '{context.GetText()}'.");
            }

            public override Func<double, double> VisitVariable([NotNull] SynthExpressionGrammarParser.VariableContext context)
            {
#if DEBUG
                var text = context.GetText();
                Debug.Assert(text == "t");
#endif
                return (double t) => t;
            }

        }

        public ExpressionParser()
        { }


        public Func<double, double> Parse(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentNullException(nameof(expression));

            expression = expression.Trim().ToLowerInvariant();

            return DoParse(expression);
        }

        Func<double, double> DoParse([NotNull]string expression)
        {
            var stream = CharStreams.fromString(expression);
            var lexer = new SynthExpressionGrammarLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SynthExpressionGrammarParser(tokens);
            parser.BuildParseTree = true;
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowingErrorListener<IToken>());

            IParseTree tree = parser.equation();
            var visitor = new SynthesizerGrammarVisitor();
            var res = tree.Accept(visitor);

            return res;
        }
    }
}
