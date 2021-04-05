using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErnstTech.SoundCore.Synthesis.Expressions.AST;

namespace ErnstTech.SoundCore.Synthesis.Expressions
{
    internal class ExpressionEvaluator : IExpressionCompiler
    {
        class Visitor : Visitor<Func<double, double>>
        {
            ExpressionEvaluator _Parent;
            public Visitor(ExpressionEvaluator parent) => _Parent = parent;

            public override Func<double, double> Visit(TimeVariableNode node) => (double t) => t;
            public override Func<double, double> Visit(NumberNode node)
            {
                double value = node.Value;
                return (double t) => value;
            }

            (Func<double, double>, Func<double,double>) GetChildren(BinaryOpNode node) => (Visit(node.Left), Visit(node.Right));

            public override Func<double, double> Visit(AddNode node)
            {
                var (left, right) = GetChildren(node);
                return (double t) => left(t) + right(t);
            }

            public override Func<double, double> Visit(SubtractNode node)
            {
                var (left, right) = GetChildren(node);
                return (double t) => left(t) - right(t);
            }

            public override Func<double, double> Visit(MultiplyNode node)
            {
                var (left, right) = GetChildren(node);
                return (double t) => left(t) * right(t);
            }

            public override Func<double, double> Visit(DivideNode node)
            {
                var (left, right) = GetChildren(node);
                return (double t) => left(t) / right(t);
            }

            public override Func<double, double> Visit(AbsoluteValueNode node)
            {
                var child = Visit(node);
                return (double t) => Math.Abs(child(t));
            }

            public override Func<double, double> Visit(NegateNode node)
            {
                var child = Visit(node);
                return (double t) => -child(t);
            }

            public override Func<double, double> Visit(FunctionNode node)
            {
                if (_Parent.Handlers.TryGetValue(node.Name, out var handler))
                {
                    var args = node.Arguments.Select(x => Visit(x)).ToList();
                    return handler.Handle(args);
                }
                throw new ArgumentException($"Unknown function: '{node.Name}'.", nameof(node));
            }
        }

        public abstract class FunctionHandler
        {
            public abstract string Name { get; }

            public virtual int MinArguments => 0;
            public virtual int MaxArguments => -1;

            public Func<double, double> Handle(List<Func<double, double>> arguments)
            {
                if (MinArguments >= 0 && arguments.Count < MinArguments)
                    throw new ArgumentException($"Function '{Name}' does not have minimum number '{MinArguments}' of arguments.", nameof(arguments));
                if (MaxArguments >= 0 && arguments.Count > MaxArguments)
                    throw new ArgumentException($"Function '{Name}' exceeds the maximum number '{MaxArguments}' of arguments.", nameof(arguments));

                return DoHandle(arguments);
            }

            protected abstract Func<double, double> DoHandle(List<Func<double, double>> arguments);
        }

        abstract class Func1Handler : FunctionHandler
        {
            public override int MinArguments => 1;
            public override int MaxArguments => 1;

            protected abstract Func<double, double> Impl { get; }

            protected override Func<double, double> DoHandle(List<Func<double, double>> arguments)
            {
                var arg = arguments.First();
                return (double t) => Impl(arg(t));
            }
        }

        abstract class Func2Handler : FunctionHandler
        {
            public override int MinArguments => 2;
            public override int MaxArguments => 2;

            protected abstract Func<double, double, double> Impl { get; }

            protected override Func<double, double> DoHandle(List<Func<double, double>> arguments)
            {
                var arg0 = arguments[0];
                var arg1 = arguments[1];

                return (double t) => Impl(arg0(t), arg1(t));
            }
        }

        class SineHandler : Func1Handler
        {
            public override string Name => "sin";
            protected override Func<double, double> Impl => Math.Sin;
        }

        class CosineHandler : Func1Handler
        {
            public override string Name => "cos";
            protected override Func<double, double> Impl => Math.Cos;
        }

        class TangentHandler : Func1Handler
        {
            public override string Name => "tan";
            protected override Func<double, double> Impl => Math.Tan;
        }

        class AbsoluteValueHandler : Func1Handler
        {
            public override string Name => "abs";
            protected override Func<double, double> Impl => Math.Abs;
        }

        class Log2Handler : Func1Handler
        {
            public override string Name => "log2";
            protected override Func<double, double> Impl => Math.Log2;
        }

        class Log10Handler : Func1Handler
        {
            public override string Name => "log10";
            protected override Func<double, double> Impl => Math.Log10;
        }

        class NaturalLogHandler : Func1Handler
        {
            public override string Name => "ln";
            protected override Func<double, double> Impl => Math.Log;
        }

        class SawHandler : Func1Handler
        {
            public override string Name => "saw";

            readonly SawWave _Wave = new();
            protected override Func<double, double> Impl => _Wave.Sample;
        }

        class TriHandler : Func1Handler
        {
            public override string Name => "tri";

            readonly TriWave _Wave = new();

            protected override Func<double, double> Impl { get => _Wave.Sample; }
        }

        class SquareWaveHandler : FunctionHandler
        {
            public override string Name => "square";
            public override int MinArguments => 1;
            public override int MaxArguments => 2;
            protected override Func<double, double> DoHandle(List<Func<double, double>> arguments)
            {
                var arg = arguments[0];
                var wave = arguments.Count == 2 ? new SquareWave(arguments[1](0)) : new SquareWave();
                return (double t) => wave.Sample(arg(t));
            }
        }

        class MovingAverageHandler : FunctionHandler
        {
            public override string Name => "movavg";
            public override int MinArguments => 2;
            public override int MaxArguments => 2;

            protected override Func<double, double> DoHandle(List<Func<double, double>> arguments)
            {
                // First argument is expected to be a constant, number of samples to average over
                var smoother = new Smoother((int)arguments[0](0));
                // Second argument is the value to be sampled & averaged.

                var arg = arguments[1];
                return (double t) => smoother.Sample(arg(t));
            }
        }

        class SquareRootHandler : Func1Handler
        {
            public override string Name => "sqrt";
            protected override Func<double, double> Impl => Math.Sqrt;
        }

        class PowerHandler : Func2Handler
        {
            public override string Name => "pow";
            protected override Func<double, double, double> Impl => Math.Pow;
        }

        class ADSRHandler : FunctionHandler
        {
            public override string Name => "adsr";
            public override int MinArguments => 0;
            public override int MaxArguments => 6;

            protected override Func<double, double> DoHandle(List<Func<double, double>> arguments)
            {
                var adsr = new ADSREnvelope();
                switch(arguments.Count)
                {
                    case 6:
                        adsr.SustainHeight = arguments[5](0);
                        goto case 5;
                    case 5:
                        adsr.AttackHeight = arguments[4](0);
                        goto case 4;
                    case 4:
                        adsr.DecayTime = arguments[3](0);
                        goto case 3;
                    case 3:
                        adsr.SustainTime = arguments[2](0);
                        goto case 2;
                    case 2:
                        adsr.ReleaseTime = arguments[1](0);
                        goto case 1;
                    case 1:
                        adsr.AttackTime = arguments[0](0);
                        goto case 0;
                    case 0:
                        break;

                    default:
                        throw new InvalidOperationException(); // unreachable
                }

                return (double t) => adsr.Factor(t);
            }
        }

        static readonly FunctionHandler[] _Builtins = new FunctionHandler[]
        {
            new SineHandler(),
            new CosineHandler(),
            new TangentHandler(),
            new AbsoluteValueHandler(),
            new Log2Handler(),
            new Log10Handler(),
            new NaturalLogHandler(),
            new SawHandler(),
            new TriHandler(),
            new SquareWaveHandler(),
            new MovingAverageHandler(),
            new PowerHandler(),
            new SquareRootHandler(),
            new ADSRHandler(),
        };

        Dictionary<string, FunctionHandler> _Handlers = _Builtins.ToDictionary(x => x.Name);
        /// <summary>
        ///     A read-only dictionary of registered function handlers.
        /// </summary>
        public IReadOnlyDictionary<string, FunctionHandler> Handlers { get => _Handlers; }

        /// <summary>
        ///     Evaluate an abstract syntax tree represented by <paramref name="abstractSyntaxTree"/>.
        /// </summary>
        /// <param name="abstractSyntaxTree">The <see cref="ExpressionNode"/> to evaluate.</param>
        /// <returns>
        ///     A <see cref="Func&lt;double, double&gt;"/> generated from <paramref name="abstractSyntaxTree"/> as a function of time.
        /// </returns>
        public Func<double, double> Compile(ExpressionNode abstractSyntaxTree) => new Visitor(this).Visit(abstractSyntaxTree);

        /// <summary>
        ///     Registers a new function handler.
        /// </summary>
        /// <param name="handler">The <see cref="FunctionHandler" /> to register.</param>
        /// <param name="overwrite">When <c>true</c>, overwrites an existing handler. Otherwise does not.</param>
        /// <returns>Returns <c>true</c> when the handler was registered, <c>false</c> otherwise.</returns>
        public bool RegisterHandler(FunctionHandler handler, bool overwrite = false)
        {
            if (_Handlers.TryAdd(handler.Name, handler))
                return true;

            if (!overwrite)
                return false;

            _Handlers[handler.Name] = handler;
            return true;
        }
    }
}
