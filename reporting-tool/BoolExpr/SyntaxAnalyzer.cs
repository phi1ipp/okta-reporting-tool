using System;
using System.Collections.Generic;

namespace reporting_tool.BoolExpr
{
    /// <summary>
    /// 
    /// </summary>
    public static class SyntaxAnalyzer
    {
        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<Tuple<GrammarElement, List<GrammarElement>>> Dictionary =>
            new List<Tuple<GrammarElement, List<GrammarElement>>>
            {
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.EXPRESSION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.BOOLEAN_EXPRESSION),
                        new Terminal(Token.TokenType.END)
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.BOOLEAN_EXPRESSION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.OPERAND),
                        new NonTerminal(GrammarToken.BINARY_OPERATION),
                        new NonTerminal(GrammarToken.OPERAND),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.BOOLEAN_EXPRESSION),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.NOT),
                        new NonTerminal(GrammarToken.OPERAND),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.BINARY_OPERATION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.OR_OPERATION),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.BINARY_OPERATION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.AND_OPERATION),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.OR_OPERATION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.OPERAND),
                        new Terminal(Token.TokenType.OR),
                        new NonTerminal(GrammarToken.OPERAND),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.AND_OPERATION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.OPERAND),
                        new Terminal(Token.TokenType.AND),
                        new NonTerminal(GrammarToken.OPERAND),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.OPERAND),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.ATTR),
                        new Terminal(Token.TokenType.EQ),
                        new Terminal(Token.TokenType.STR),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.OPERAND),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.ATTR),
                        new Terminal(Token.TokenType.PR),
                    }),
                new Tuple<GrammarElement, List<GrammarElement>>(
                    new NonTerminal(GrammarToken.OPERAND),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.OPEN),
                        new NonTerminal(GrammarToken.BOOLEAN_EXPRESSION),
                        new Terminal(Token.TokenType.CLOSE),
                    }),
            };

        public static Tree<GrammarElement> Parse(IEnumerable<Token> lstTokens)
        {
            
        }
    }


    public abstract class GrammarElement : object
    {
        public abstract override string ToString();
    }

    public enum GrammarToken
    {
        EXPRESSION,
        BOOLEAN_EXPRESSION,
        BINARY_OPERATION,
        OR_OPERATION,
        AND_OPERATION,
        OPERAND,
    }

    public class Terminal : GrammarElement
    {
        public Token.TokenType type { get; }
        public string value { get; }

        public Terminal(Token.TokenType type, string value = "")
        {
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            return $"[{type}:{value}]";
        }
    }

    public class NonTerminal : GrammarElement
    {
        public GrammarToken type { get; }

        public NonTerminal(GrammarToken token)
        {
            type = token;
        }

        public override string ToString()
        {
            return $"{type}";
        }
    }

    public class Tree<T>
    {
        public T data { get; }
        public Tree<T> left { get; set; }
        public Tree<T> right { get; set; }
        
        public Tree(T data)
        {
            this.data = data;
        }
        
    }
}