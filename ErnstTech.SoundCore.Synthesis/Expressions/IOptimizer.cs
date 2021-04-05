using System;

namespace ErnstTech.SoundCore.Synthesis.Expressions
{
    interface IOptimizer
    {
        AST.ExpressionNode Optimize(AST.ExpressionNode expression);
    }
}
