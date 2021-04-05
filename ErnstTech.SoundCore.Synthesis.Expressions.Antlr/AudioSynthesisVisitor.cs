using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using ErnstTech.SoundCore.Synthesis.Expressions.AST;

namespace ErnstTech.SoundCore.Synthesis.Expressions.Antlr
{
    public class AudioSynthesisVisitor : AudioSynthesisGrammarBaseVisitor<ExpressionNode>
    {
        public override ExpressionNode VisitTimeVarExpr([NotNull] AudioSynthesisGrammarParser.TimeVarExprContext context)
        {
            return new TimeVariableNode();
        }

        public override ExpressionNode VisitSubExpr([NotNull] AudioSynthesisGrammarParser.SubExprContext context)
        {
            return Visit(context.inner);
        }

        public override ExpressionNode VisitUnaryExpr([NotNull] AudioSynthesisGrammarParser.UnaryExprContext context)
        {
            var child = Visit(context.child);

            switch(context.op.Type)
            {
                case AudioSynthesisGrammarLexer.OP_ADD:
                    return new AbsoluteValueNode(child);

                case AudioSynthesisGrammarLexer.OP_SUB:
                    return new NegateNode(child);

                default:
                    throw new ArgumentException($"Unexpected 'op' type: {context.op.Type}", nameof(context));
            }
        }

        public override ExpressionNode VisitBinaryExpr([NotNull] AudioSynthesisGrammarParser.BinaryExprContext context)
        {
            var left = Visit(context.leftChild);
            var right = Visit(context.rightChild);

            switch(context.op.Type)
            {
                case AudioSynthesisGrammarLexer.OP_ADD:
                    return new AddNode(left, right);

                case AudioSynthesisGrammarLexer.OP_SUB:
                    return new SubtractNode(left, right);

                case AudioSynthesisGrammarLexer.OP_MUL:
                    return new MultiplyNode(left, right);

                case AudioSynthesisGrammarLexer.OP_DIV:
                    return new DivideNode(left, right);

                case AudioSynthesisGrammarLexer.OP_EXP:
                    return new ExponentiationNode(left, right);

                default:
                    throw new ArgumentException($"Unexpected 'op': {context.op.Type}", nameof(context));
            }
        }

        public override ExpressionNode VisitValueExpr([NotNull] AudioSynthesisGrammarParser.ValueExprContext context)
        {
            var value = double.Parse(context.value.Text);
            return new NumberNode(value);
        }

        public override ExpressionNode VisitEulerExpr([NotNull] AudioSynthesisGrammarParser.EulerExprContext context) => new EulerNode();
        public override ExpressionNode VisitPiExpr([NotNull] AudioSynthesisGrammarParser.PiExprContext context) => new PiNode();

        public override ExpressionNode VisitExprList([NotNull] AudioSynthesisGrammarParser.ExprListContext context)
        {
            var children = context.expr().Select(x => Visit(x)).ToList();
            return new ExpressionListNode(children);
        }
        public override ExpressionNode VisitFunctionExpr([NotNull] AudioSynthesisGrammarParser.FunctionExprContext context)
        {
            var name = context.funcName.Text;
            var children = context.arguments is null
                ? new List<ExpressionNode>()
                : ((ExpressionListNode)Visit(context.arguments)).Children;

            return new FunctionNode(name, children);
        }

        public override ExpressionNode VisitCompileUnit([NotNull] AudioSynthesisGrammarParser.CompileUnitContext context)
        {
            return Visit(context.finalUnit);
        }
    }
}
