using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions.AST
{
    public abstract class Visitor<T>
    {
        // Time Variable
        public abstract T Visit(TimeVariableNode node);

        // Literals
        public abstract T Visit(NumberNode node);

        // Binary Operations
        public abstract T Visit(AddNode node);
        public abstract T Visit(SubtractNode node);
        public abstract T Visit(MultiplyNode node);
        public abstract T Visit(DivideNode node);

        // Unary Operations
        public abstract T Visit(AbsoluteValueNode node);
        public abstract T Visit(NegateNode node);

        // Function
        public abstract T Visit(FunctionNode node);

        public T Visit(ExpressionNode node) => Visit((dynamic)node);
    }
}
