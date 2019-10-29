using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class representing a filter of user entries for reports
    /// </summary>
    public class UserFilter
    {
        /// <summary>
        /// Filtering function composed
        /// </summary>
        public Func<IUser, bool> F { get; }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="expression">String representing a conditional expression</param>
        public UserFilter(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                F = user => true;
                return;
            }

            var lexer = new BoolExprLexer(CharStreams.fromstring(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(ThrowingErrorListener.Instance);

            var tokens = new CommonTokenStream(lexer);
            var parser = new BoolExprParser(tokens) {BuildParseTree = true};
            parser.RemoveErrorListeners();
            parser.AddErrorListener(ThrowingErrorListener.Instance);

            var tree = parser.expr();
            var visitor = new BoolExprVisitor();

            F = visitor.Visit(tree);
        }
    }

    internal class BoolExprVisitor : BoolExprBaseVisitor<Func<IUser, bool>>
    {
        public override Func<IUser, bool> VisitNotExp(BoolExprParser.NotExpContext context)
        {
            var f = Visit(context.children.Last());

            return user => !f(user);
        }

        public override Func<IUser, bool> VisitOrExp(BoolExprParser.OrExpContext context)
        {
            var left = Visit(context.children.First());
            var right = Visit(context.children.Last());

            return user => left(user) || right(user);
        }

        public override Func<IUser, bool> VisitAndExp(BoolExprParser.AndExpContext context)
        {
            var left = Visit(context.children.First());
            var right = Visit(context.children.Last());

            return user => left(user) && right(user);
        }

        public override Func<IUser, bool> VisitAttr_pr(BoolExprParser.Attr_prContext context)
        {
            var (attrType, attrName) = GetAttributeInfo(context.children.First());

            return user =>
                attrType == "profile"
                    ? !string.IsNullOrEmpty(user.Profile[attrName]?.ToString())
                    : !string.IsNullOrEmpty(user.GetNonProfileAttribute(attrName));
        }

        public override Func<IUser, bool> VisitEqCompare(BoolExprParser.EqCompareContext context)
        {
            var attrVal = context.children.Last().GetText().Trim('"');

            var (attrType, attrName) = GetAttributeInfo(context.children.First());

            return user =>
                attrType == "profile"
                    ? user.Profile[attrName]?.ToString() == attrVal
                    : user.GetNonProfileAttribute(attrName) == attrVal;
        }

        public override Func<IUser, bool> VisitSwCompare(BoolExprParser.SwCompareContext context)
        {
            var attrVal = context.children.Last().GetText().Trim('"');

            var (attrType, attrName) = GetAttributeInfo(context.children.First());

            return user =>
                attrType == "profile"
                    ? (user.Profile[attrName] != null && user.Profile[attrName].ToString().StartsWith(attrVal))
                    : user.GetNonProfileAttribute(attrName).StartsWith(attrVal);
        }

        public override Func<IUser, bool> VisitCoCompare(BoolExprParser.CoCompareContext context)
        {
            var attrVal = context.children.Last().GetText().Trim('"');

            var (attrType, attrName) = GetAttributeInfo(context.children.First());

            switch (attrType)
            {
                case "profile":
                    return user =>
                    {
                        if (user.Profile[attrName] is IEnumerable<object> coll)
                        {
                            return string.Join(',', coll.Select(val => val.ToString()))
                                .Contains(attrVal);
                        }
                        else
                        {
                            return user.Profile[attrName]?.ToString().Contains(attrVal) ?? false;
                        }
                    };
                default: return user => user.GetNonProfileAttribute(attrName).Contains(attrVal);
            }
        }

        public override Func<IUser, bool> VisitParenthesisExp(BoolExprParser.ParenthesisExpContext context) =>
            Visit(context.children[1]);

        private static Tuple<string, string> GetAttributeInfo(IParseTree node)
        {
            return node is BoolExprParser.ProfileAttrContext
                ? new Tuple<string, string>("profile", node.GetText().Substring("profile.".Length))
                : new Tuple<string, string>("non-profile", node.GetText());
        }
    }

    internal class ThrowingErrorListener : IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
    {
        public static ThrowingErrorListener Instance => new ThrowingErrorListener();

        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line,
            int charPositionInLine,
            string msg, RecognitionException e)
        {
            throw new ParseCanceledException($"line {line}: {charPositionInLine} {msg}");
        }

        public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine,
            string msg, RecognitionException e)
        {
            throw new ParseCanceledException($"line {line}: {charPositionInLine} {msg}");
        }
    }
}