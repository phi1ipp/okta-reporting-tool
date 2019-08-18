using System;
using System.Collections.Generic;
using System.Linq;

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
        public static IEnumerable<Rule> Dictionary =>
            new List<Rule>
            {
                new Rule(
                    new NonTerminal(GrammarToken.EXPRESSION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.EXPRESSION),
                        new Terminal(Token.TokenType.OR),
                        new NonTerminal(GrammarToken.EXPRESSION),
                    }),
                new Rule(
                    new NonTerminal(GrammarToken.EXPRESSION),
                    new List<GrammarElement>
                    {
                        new NonTerminal(GrammarToken.EXPRESSION),
                        new Terminal(Token.TokenType.AND),
                        new NonTerminal(GrammarToken.EXPRESSION),
                    }),
                new Rule(
                    new NonTerminal(GrammarToken.EXPRESSION),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.NOT),
                        new NonTerminal(GrammarToken.EXPRESSION),
                    }),
                new Rule(
                    new NonTerminal(GrammarToken.EXPRESSION),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.OPEN),
                        new NonTerminal(GrammarToken.EXPRESSION),
                        new Terminal(Token.TokenType.CLOSE),
                    }),
                new Rule(
                    new NonTerminal(GrammarToken.EXPRESSION),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.ATTR),
                        new Terminal(Token.TokenType.EQ),
                        new Terminal(Token.TokenType.STR),
                    }),
                new Rule(
                    new NonTerminal(GrammarToken.EXPRESSION),
                    new List<GrammarElement>
                    {
                        new Terminal(Token.TokenType.ATTR),
                        new Terminal(Token.TokenType.PR),
                    }),
            };

        public static bool Match(IEnumerable<Token> lst) => Dictionary.Any(rule => rule.Match(lst));

//        public static Tree<GrammarElement> Parse(IEnumerable<Token> lstTokens) {
//                return new Tree<GrammarElement>(new );
//        }
    }

    public class Rule
    {
        public GrammarElement left;
        public IEnumerable<GrammarElement> right;

        public Rule(GrammarElement left, IEnumerable<GrammarElement> right)
        {
            this.left = left;
            this.right = right;
        }

        public bool Match(IEnumerable<Token> lstTokens) =>
            Match(lstTokens.Select(t => new Terminal(t.Type, t.Value)));

        public bool Match(IEnumerable<GrammarElement> lst) =>
            right
                .Zip(lst, (element, grammarElement) =>
                    element.GetElementType() == grammarElement.GetElementType() ||
                    element is NonTerminal && SyntaxAnalyzer.Dictionary.Any(rule => rule.reduce()))
                .All(el => el);

        public GrammarElement reduce(IEnumerable<GrammarElement> lst) => Match(lst) ? left : null;
    }

    public abstract class GrammarElement : object
    {
        public abstract override string ToString();
        public abstract string GetElementType();
    }

    public enum GrammarToken
    {
        EXPRESSION,
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

        public override string GetElementType() => type.ToString();
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

        public override string GetElementType() => type.ToString();
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