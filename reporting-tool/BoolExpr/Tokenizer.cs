using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// 
    /// </summary>
    public class Tokenizer
    {
        public static Dictionary<Token.TokenType, string> Dictionary
            = new Dictionary<Token.TokenType, string>
            {
                {Token.TokenType.OPEN, "("},
                {Token.TokenType.CLOSE, ")"},
                {Token.TokenType.EQ, "eq "},
                {Token.TokenType.AND, "and "},
                {Token.TokenType.OR, "or "},
                {Token.TokenType.PR, "pr "},
                {Token.TokenType.NOT, "not "},
                {Token.TokenType.STR, "\""},
                {Token.TokenType.ATTR, "profile."},
            };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Func<IUser, bool> GetUserFilter(string expression)
        {
            return user => true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<Token> Tokenize(string expression)
        {
            var (token, expr) = expression.GetNextToken();

            if (token.Type == Token.TokenType.END)
                return Enumerable.Empty<Token>();

            return new List<Token>
            {
                token
            }.Concat(Tokenize(expr));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 
        /// </summary>
        public enum TokenType
        {
            END,
            OPEN,
            CLOSE,
            EQ,
            PR,
            NOT,
            STR,
            ATTR,
            AND,
            OR,
        }

        public TokenType Type { get; }
        public string Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Token(TokenType type, string value)
        {
            Value = value;
            Type = type;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class StringExtension
    {
        public static Tuple<Token, string> GetNextToken(this string expression)
        {
            var tokenType = Tokenizer.Dictionary.Keys
                .FirstOrDefault(expression.StartsWith);

            switch (tokenType)
            {
                case Token.TokenType.ATTR:
                    var attr = expression.GetAttrName();

                    return new Tuple<Token, string>(
                        new Token(Token.TokenType.ATTR, attr),
                        expression.TrimStart()
                            .Substring(attr.Length + Tokenizer.Dictionary[Token.TokenType.ATTR].Length));

                case Token.TokenType.STR:
                    var literal = expression.GetStringLiteral();

                    return new Tuple<Token, string>(
                        new Token(Token.TokenType.STR, literal),
                        expression.TrimStart().Substring(literal.Length + 2)
                    );

                case Token.TokenType.OPEN:
                case Token.TokenType.CLOSE:
                    return new Tuple<Token, string>(
                        new Token(tokenType, Tokenizer.Dictionary[tokenType]),
                        expression.TrimStart().Substring(Tokenizer.Dictionary[tokenType].Length));

                case Token.TokenType.EQ:
                case Token.TokenType.PR:
                case Token.TokenType.NOT:
                case Token.TokenType.AND:
                case Token.TokenType.OR:
                    return new Tuple<Token, string>(
                        new Token(tokenType, Tokenizer.Dictionary[tokenType]),
                        expression.TrimStart().Substring(Tokenizer.Dictionary[tokenType].Length - 1));

                case Token.TokenType.END:
                    return new Tuple<Token, string>(
                        new Token(Token.TokenType.END, ""), "");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        public static bool StartsWith(this string str, Token.TokenType tokenType)
        {
            return str.ToLower().TrimStart().StartsWith(Tokenizer.Dictionary[tokenType])
                || str.ToLower().Trim() == Tokenizer.Dictionary[tokenType].Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStringLiteral(this string str)
        {
            var match = Regex.Match(str, "^\\s*\"([^\"]+)\"");

            return match.Success ? match.Groups[1].ToString() : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetAttrName(this string str)
        {
            var match = Regex.Match(str, "^\\s*profile.([a-zA-Z0-9]+)");

            return match.Success ? match.Groups[1].ToString() : null;
        }
    }
}