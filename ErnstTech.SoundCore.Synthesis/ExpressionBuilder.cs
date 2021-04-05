using System;
using ErnstTech.SoundCore.Synthesis.Expressions;
using ErnstTech.SoundCore.Synthesis.Expressions.Optimizations;

namespace ErnstTech.SoundCore.Synthesis
{
    public class ExpressionBuilder
    {
        readonly IExpressionParser _Parser;
        readonly IExpressionCompiler _Compiler;
        readonly IOptimizer[] _Optimizers = new IOptimizer[]
        {
            new ConstantFolder(),
        };

        public ExpressionBuilder(IExpressionParser parser, IExpressionCompiler? compiler = null)
        {
            _Parser = parser;
            _Compiler = compiler ?? new ExpressionEvaluator();
        }

        public Func<double, double> Compile(string expression)
        {
            var tree = _Parser.Parse(expression);

            foreach (var opt in _Optimizers)
                tree = opt.Optimize(tree);

            return _Compiler.Compile(tree);
        }
    }
}
