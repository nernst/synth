using ErnstTech.SoundCore.Synthesis.Expressions.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ErnstTech.SoundCore.Synthesis.Expressions
{
    public class CompilationError : Exception
    {
       public CompilationError() : base() { }
       public CompilationError(string message) : base(message) { }
    }

    internal class Compiler : IExpressionCompiler
    {
        class Visitor : Visitor<Expression>
        {
            class DependsOnTimeVarVisitor : Visitor<bool>
            {
                public override bool Visit(TimeVariableNode node) => true;
                public override bool Visit(NumberNode node) => false;

                bool VisitBinary(BinaryOpNode node) => Visit(node.Left) || Visit(node.Right);

                public override bool Visit(AddNode node) => VisitBinary(node);
                public override bool Visit(SubtractNode node) => VisitBinary(node);

                public override bool Visit(MultiplyNode node) => VisitBinary(node);

                public override bool Visit(DivideNode node) => VisitBinary(node);

                public override bool Visit(ExponentiationNode node) => VisitBinary(node);

                public override bool Visit(AbsoluteValueNode node) => Visit(node.InnerNode);

                public override bool Visit(NegateNode node) => Visit(node.InnerNode);


                public override bool Visit(FunctionNode node) => node.Arguments.Select(x => Visit(x)).Any();
            }

            readonly DependsOnTimeVarVisitor dependsVisitor = new ();
            readonly ParameterExpression timeVar;
            readonly IList<Tuple<ParameterExpression, Expression>> closures = new List<Tuple<ParameterExpression, Expression>>();

            ParameterExpression AddClosure(Expression initExpression, Type type)
            {
                var param = Expression.Parameter(type, $"closure{closures.Count}");
                closures.Add(Tuple.Create(param, initExpression));
                return param;
            }

            void EnsureConstant(string methodName, int argNum, ExpressionNode node)
            {
                if (dependsVisitor.Visit(node))
                    throw new CompilationError($"Function {methodName} argument {argNum} cannot depend on 't'.");
            }

            internal Visitor() 
            {
                timeVar = Expression.Parameter(typeof(double), "time"); 
            }

            static string TypesToString(Type[] types) => string.Join(", ", types.Select(t => t.Name));

            static MethodInfo GetMethod(Type target, string methodName, params Type[] args) => 
                target.GetMethod(methodName, args) 
                ?? throw new InvalidOperationException(
                    $"Could not get method '{methodName}' with arguments [{TypesToString(args)}]."
                    );

            static ConstructorInfo GetConstructor(Type target, params Type[] args) => 
                target.GetConstructor(args) 
                ?? throw new InvalidOperationException(
                    $"Could not get constructor for '{target.Name}' with arguments [{TypesToString(args)}]."
                    );

            static MemberInfo GetMember(Type target, string name) => 
                target.GetMember(name).FirstOrDefault() 
                ?? throw new InvalidOperationException($"Could not get member '{name}' on type '{target.Name}'.");

            public override Expression Visit(TimeVariableNode node)
            {
                return timeVar;
            }

            public override Expression Visit(NumberNode node)
            {
                return Expression.Constant(node.Value);
            }

            (Expression, Expression) GetChildren(BinaryOpNode node) => (Visit(node.Left), Visit(node.Right));

            public override Expression Visit(AddNode node)
            {
                var (left, right) = GetChildren(node);
                return Expression.Add(left, right);
            }

            public override Expression Visit(SubtractNode node)
            {
                var (left, right) = GetChildren(node);
                return Expression.Subtract(left, right);
            }

            public override Expression Visit(MultiplyNode node)
            {
                var (left, right) = GetChildren(node);
                return Expression.Multiply(left, right);
            }

            public override Expression Visit(DivideNode node)
            {
                var (left, right) = GetChildren(node);
                return Expression.Divide(left, right);
            }

            public override Expression Visit(ExponentiationNode node)
            {
                var (left, right) = GetChildren(node);
                var method = GetMethod(typeof(Math), "Pow", typeof(double), typeof(double));

                return Expression.Call(
                    null,
                    method,
                    left,
                    right
                   );
            }

            public override Expression Visit(AbsoluteValueNode node)
            {
                var child = Visit(node.InnerNode);
                var method = GetMethod(typeof(Math), "Abs", typeof(double));

                return Expression.Call(
                    null,
                    method,
                    child
                   );
            }

            public override Expression Visit(NegateNode node)
            {
                var child = Visit(node.InnerNode);
                return Expression.Negate(child);
            }

            public override Expression Visit(FunctionNode node)
            {
                switch(node.Name)
                {
                    // Closures
                    case "adsr": return HandleADSRClosure(node);
                    case "moveavg": return HandleMovingAverageClosure(node);
                    case "saw": return HandleSimpleClosure<SawWave>(node);
                    case "square": return HandleSimpleClosure<SquareWave>(node);
                    case "tri": return HandleSimpleClosure<TriWave>(node);

                    // 1-argument math functions
                    case "sin": return HandleMathFunc1(node, "Sin");
                    case "cos": return HandleMathFunc1(node, "Cos");
                    case "tan": return HandleMathFunc1(node, "Tan");
                    case "abs": return HandleMathFunc1(node, "Abs");
                    case "log10": return HandleMathFunc1(node, "Log10");
                    case "log2": return HandleMathFunc1(node, "Log2");
                    case "ln": return HandleMathFunc1(node, "Log");
                    case "sqrt": return HandleMathFunc1(node, "Sqrt");

                    // 2-argument math functions
                    case "pow": return HandleMathFunc2(node, "Pow");
                    
                    default:
                        throw new CompilationError($"Unknown or unsupported function: '{node.Name}'.");
                }

                throw new NotImplementedException();
            }

            Expression HandleSimpleClosure<ClosureType>(FunctionNode node) where ClosureType : IGenerator, new()
            {
                if (node.Arguments.Count != 1)
                    throw new CompilationError($"Function '{node.Name}' takes 1 argument.");

                var arg = Visit(node.Arguments[0]);
                var initExpression = Expression.New(typeof(ClosureType));
                var param = AddClosure(initExpression, typeof(ClosureType));
                var method = GetMethod(typeof(ClosureType), "Generate", typeof(double));

                return Expression.Call(param, method, arg);
            }

            Expression HandleMathFunc1(FunctionNode node, string methodName)
            {
                if (node.Arguments.Count != 1)
                    throw new CompilationError($"Function '{node.Name}' takes 1 argument.");

                var method = GetMethod(typeof(Math), methodName, typeof(double));
                return Expression.Call(null, method, Visit(node.Arguments[0]));
            }

            Expression HandleMathFunc2(FunctionNode node, string methodName)
            {
                if (node.Arguments.Count != 2)
                    throw new CompilationError($"Function '{node.Name}' takes 2 arguments.");

                var method = GetMethod(typeof(Math), methodName, typeof(double), typeof(double));
                return Expression.Call(null, method, Visit(node.Arguments[0]), Visit(node.Arguments[1]));
            }

            static readonly string[] ADSRMemberNames = new [] {
                "AttackTime",
                "ReleaseTime",
                "SustainTime",
                "DecayTime",
                "AttackHeight",
                "SustainHeight",
            };

            Expression HandleADSRClosure(FunctionNode node)
            {
                if (node.Arguments.Count > ADSRMemberNames.Length)
                        throw new CompilationError($"Function '{node.Name}' takes between 0 and {ADSRMemberNames.Length} arguments.");

                var type = typeof(ADSREnvelope);
                List<MemberBinding> members = new List<MemberBinding>();
                for (int i = 0; i < node.Arguments.Count; ++i)
                {
                    EnsureConstant(node.Name, i, node.Arguments[i]);
                    members.Add(Expression.Bind(GetMember(type, ADSRMemberNames[i]), Visit(node.Arguments[i])));
                }
                var initExpression = Expression.MemberInit(
                    Expression.New(type),
                    members);

                var param = AddClosure(initExpression, type);
                var method = GetMethod(type, "Generate", typeof(double));
                return Expression.Call(param, method, timeVar);
            }

            Expression HandleMovingAverageClosure(FunctionNode node)
            {
                if (node.Arguments.Count != 2)
                    throw new CompilationError($"Function '{node.Name}' takes exactly 2 arguments.");

                EnsureConstant(node.Name, 0, node.Arguments[0]);

                var first = Visit(node.Arguments[0]);
                var second = Visit(node.Arguments[1]);
                var initExpression = Expression.New(GetConstructor(typeof(Smoother), new Type[] { typeof(double) }), first);
                var closure = AddClosure(initExpression, typeof(Smoother));
                var method = GetMethod(typeof(Smoother), "Sample", typeof(double));
                return Expression.Call(closure, method, second);
            }

            public Func<double, double> Compile(ExpressionNode node)
            {
                var body = Visit(node);

                List<ParameterExpression> parameters = new List<ParameterExpression>(closures.Count + 1) { timeVar, };
                List<Expression> expressions = new List<Expression>();

                foreach (var tup in closures)
                {
                    parameters.Add(tup.Item1);
                    expressions.Add(Expression.Assign(tup.Item1, tup.Item2));
                }

                expressions.Add(Expression.Lambda<Func<double, double>>(body, timeVar));

                var block = Expression.Block(parameters, expressions);
                var closure = Expression.Lambda<Func<Func<double, double>>>(block).Compile();
                return closure();
            }
        }

        public Func<double, double> Compile(ExpressionNode tree) => new Visitor().Compile(tree);
    }
}
