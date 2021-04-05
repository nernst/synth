using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ErnstTech.SoundCore.Synthesis.Expressions.AST;

namespace ErnstTech.SoundCore.Synthesis.Expressions.Antlr
{
    public class ParseError : Exception
    {
        public ParseError() : base()
        { }

        public ParseError(string message) : base(message)
        { }
    }

    internal class ThrowingErrorListener<TSymbol> : IAntlrErrorListener<TSymbol>
    {
        #region IAntlrErrorListener<TSymbol> Implementation
        public void SyntaxError(TextWriter output, IRecognizer recognizer, TSymbol offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ParseError($"line {line}:{charPositionInLine} {msg}");
        }
        #endregion
    }

    public class ExpressionParser : IExpressionParser
    {
        public ExpressionNode Parse(string expression)
        {
            var stream = CharStreams.fromString(expression);
            var lexer = new AudioSynthesisGrammarLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new AudioSynthesisGrammarParser(tokens);
            parser.BuildParseTree = true;
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowingErrorListener<IToken>());

            IParseTree tree = parser.compileUnit();
            var visitor = new AudioSynthesisVisitor();
            var res = tree.Accept(visitor);
            if (res is null)
                throw new ParseError("Unexpected null building AST.");

            return res;
        }
    }
}
