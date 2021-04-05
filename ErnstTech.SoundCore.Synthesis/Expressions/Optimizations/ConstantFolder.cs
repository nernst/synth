using ErnstTech.SoundCore.Synthesis.Expressions.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis.Expressions.Optimizations
{
    internal class ConstantFolder : IOptimizer
    {
        public ExpressionNode Optimize(ExpressionNode expression)
        {
            return expression switch
            {
                UnaryOpNode unaryOp => Optimize(unaryOp),
                BinaryOpNode binOp => Optimize(binOp),
                FunctionNode func => Optimize(func),
                _ => expression
            };
        }

        ExpressionNode Optimize(UnaryOpNode unaryOpNode)
        {
            unaryOpNode.InnerNode = Optimize(unaryOpNode.InnerNode);

            if (unaryOpNode.InnerNode is NumberNode numberNode)
            {
                double result;

                switch(unaryOpNode)
                {
                    case NegateNode:
                        result = -numberNode.Value;
                        break;

                    case AbsoluteValueNode:
                        result = Math.Abs(numberNode.Value);
                        break;


                    default:
                        throw new InvalidOperationException($"Unexpected UnaryOpNode type: {unaryOpNode.GetType().Name}");
                }

                return new NumberNode(result);
            }

            return unaryOpNode;
        }

        ExpressionNode Optimize(BinaryOpNode binaryOpNode)
        {
            binaryOpNode.Left = Optimize(binaryOpNode.Left);
            binaryOpNode.Right = Optimize(binaryOpNode.Right);

            if (binaryOpNode.Left is NumberNode lhs && binaryOpNode.Right is NumberNode rhs)
            {
                var left = lhs.Value;
                var right = rhs.Value;
                double result;

                switch(binaryOpNode)
                {
                    case AddNode:
                        result = left + right;
                        break;

                    case SubtractNode:
                        result = left - right;
                        break;

                    case MultiplyNode:
                        result = left * right;
                        break;

                    case DivideNode:
                        result = left / right;
                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected BinaryOpNode type: {binaryOpNode.GetType().Name}");
                }

                return new NumberNode(result);
            }

            return binaryOpNode;
        }

        ExpressionNode Optimize(FunctionNode functionNode)
        {
            functionNode.Arguments = functionNode.Arguments.Select(x => Optimize(x)).ToList();

            return functionNode;
        }
    }
}
