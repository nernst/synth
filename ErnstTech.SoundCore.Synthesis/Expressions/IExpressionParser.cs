using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions
{
    /// <summary>
    ///     Interface for an audio synthesis expression parser. 
    ///     Taking a mathematical expression creating an abstract syntax tree.
    /// </summary>
    public interface IExpressionParser
    {
        AST.ExpressionNode Parse(string expression);
    }
}
