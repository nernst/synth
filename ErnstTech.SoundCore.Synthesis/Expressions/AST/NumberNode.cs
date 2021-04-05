using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions.AST
{
    public class NumberNode : ExpressionNode
    {
        public double Value { get; init; }

        public NumberNode(double value) => Value = value;
    }

    public class PiNode : NumberNode
    {
        public PiNode() : base(Math.PI) { }
    }

    public class EulerNode : NumberNode
    {
        public EulerNode() : base(Math.E) { }
    }
}
