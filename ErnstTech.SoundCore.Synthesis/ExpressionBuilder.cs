using System;
using ErnstTech.SoundCore.Synthesis.Expressions;

namespace ErnstTech.SoundCore.Synthesis
{
    public class ExpressionBuilder
    {
        IExpressionParser _Parser;
        IExpressionCompiler _Compiler;

        public ExpressionBuilder(IExpressionParser parser, IExpressionCompiler? compiler = null)
        {
            _Parser = parser;
            _Compiler = compiler ?? new ExpressionEvaluator();
        }

        public Func<double, double> Compile(string expression)
        {
            var tree = _Parser.Parse(expression);
            return _Compiler.Compile(tree);
        }
    }
}
