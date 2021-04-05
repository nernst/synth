using System;
using ErnstTech.SoundCore.Synthesis.Expressions;
using ErnstTech.SoundCore.Synthesis.Expressions.Optimizations;

namespace ErnstTech.SoundCore.Synthesis
{
    public class ExpressionBuilder
    {
        private readonly IExpressionParser _Parser;
        private readonly IExpressionCompiler _Compiler;
        private static readonly IOptimizer[] optimizers = new IOptimizer[]
                {
                    new ConstantFolder(),
                };
        readonly IOptimizer[] _Optimizers = optimizers;

        public ExpressionBuilder(IExpressionParser parser, IExpressionCompiler? compiler = null)
        {
            _Parser = parser;
            _Compiler = compiler ?? new ExpressionEvaluator();
        }

        public Func<double, double> Compile(string expression)
        {
            expression = expression.ToLower();
            var tree = _Parser.Parse(expression);

            foreach (var opt in _Optimizers)
                tree = opt.Optimize(tree);

            return _Compiler.Compile(tree);
        }
    }
}
