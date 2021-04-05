using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions.AST
{
    public abstract class BinaryOpNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }

        public BinaryOpNode(ExpressionNode left, ExpressionNode right)
        {
            Left = left;
            Right = right;
        }
    }

    public class AddNode : BinaryOpNode
    {
        public AddNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
    }

    public class SubtractNode : BinaryOpNode
    {
        public SubtractNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
    }

    public class MultiplyNode : BinaryOpNode
    {
        public MultiplyNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
    }

    public class DivideNode : BinaryOpNode
    {
        public DivideNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
    }

    public class ExponentiationNode : BinaryOpNode
    {
        public ExponentiationNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
    }
}
