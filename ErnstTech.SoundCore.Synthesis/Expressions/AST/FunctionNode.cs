using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions.AST
{
    public class FunctionNode : ExpressionNode
    {
        public string Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        public FunctionNode(string name, List<ExpressionNode> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}
