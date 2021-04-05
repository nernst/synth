using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions.AST
{
    public abstract class UnaryOpNode : ExpressionNode
    {
        public ExpressionNode InnerNode { get; set; }

        public UnaryOpNode(ExpressionNode innerNode)
        {
            InnerNode = innerNode;
        }
    }

    public class NegateNode : UnaryOpNode
    {
        public NegateNode(ExpressionNode innerNode) : base(innerNode) { }
    }

    public class AbsoluteValueNode : UnaryOpNode
    {
        public AbsoluteValueNode(ExpressionNode innerNode) : base(innerNode) { }
    }
}

