using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions
{
    public interface IExpressionCompiler
    {
        Func<double, double> Compile(AST.ExpressionNode tree);
    }
}
