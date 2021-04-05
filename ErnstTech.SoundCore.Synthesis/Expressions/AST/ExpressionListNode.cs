using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions.AST
{
    public class ExpressionListNode : ExpressionNode
    {
        public List<ExpressionNode> Children { get; set; }

        public ExpressionListNode(List<ExpressionNode> children) => Children = children;
    }
}
