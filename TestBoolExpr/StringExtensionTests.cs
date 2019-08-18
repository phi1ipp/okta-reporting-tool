using System;
using reporting_tool;
using Xunit;

namespace TestBoolExpr
{
    public class StringExtensionTests
    {
        [Fact]
        public void StartsWithOpen()
        {
            Assert.True("(abc".StartsWith(Token.TokenType.OPEN));
            Assert.True("  (abc".StartsWith(Token.TokenType.OPEN));
            Assert.False(" )  (abc".StartsWith(Token.TokenType.OPEN));
        }
        
        [Fact]
        public void StartsWithClose()
        {
            Assert.False("(abc".StartsWith(Token.TokenType.CLOSE));
            Assert.False("  (abc".StartsWith(Token.TokenType.CLOSE));
            Assert.True(" )  (abc".StartsWith(Token.TokenType.CLOSE));
        }
        
        [Fact]
        public void StartsWithNot()
        {
            Assert.True("not (abc".StartsWith(Token.TokenType.NOT));
            Assert.True(" Not (abc".StartsWith(Token.TokenType.NOT));
            Assert.True(" nOt )  (abc".StartsWith(Token.TokenType.NOT));
            Assert.False(" enOt )  (abc".StartsWith(Token.TokenType.NOT));
            Assert.False(" nOte )  (abc".StartsWith(Token.TokenType.NOT));
        }
        
        [Fact]
        public void StartsWithEqual()
        {
            Assert.True("eq (abc".StartsWith(Token.TokenType.EQ));
            Assert.True(" eQ (abc".StartsWith(Token.TokenType.EQ));
            Assert.True(" Eq )  (abc".StartsWith(Token.TokenType.EQ));
            Assert.False(" Equal )  (abc".StartsWith(Token.TokenType.EQ));
        }
        
        [Fact]
        public void StartsWithOr()
        {
            Assert.True("or (abc".StartsWith(Token.TokenType.OR));
            Assert.True(" oR (abc".StartsWith(Token.TokenType.OR));
            Assert.True(" OR )  (abc".StartsWith(Token.TokenType.OR));
            Assert.False(" opra )  (abc".StartsWith(Token.TokenType.OR));
        }
        
        [Fact]
        public void StartsWithAnd()
        {
            Assert.True("and (abc".StartsWith(Token.TokenType.AND));
            Assert.True(" aNd (abc".StartsWith(Token.TokenType.AND));
            Assert.True(" AND )  (abc".StartsWith(Token.TokenType.AND));
            Assert.False(" opra )  (abc".StartsWith(Token.TokenType.AND));
        }
        
        [Fact]
        public void StartsWithPr()
        {
            Assert.True("pr (abc".StartsWith(Token.TokenType.PR));
            Assert.True(" pR (abc".StartsWith(Token.TokenType.PR));
            Assert.True(" PR )  (abc".StartsWith(Token.TokenType.PR));
            Assert.False(" opra )  (abc".StartsWith(Token.TokenType.PR));
        }
        
        [Fact]
        public void StartsWithStr()
        {
            Assert.True("\"pr (abc".StartsWith(Token.TokenType.STR));
            Assert.True(" \"pR (abc".StartsWith(Token.TokenType.STR));
            Assert.False(" P\"R )  (abc".StartsWith(Token.TokenType.STR));
            Assert.False(" opra )  (abc".StartsWith(Token.TokenType.STR));
        }
        
        [Fact]
        public void StartsWithAttr()
        {
            Assert.True("profile.LOA (abc".StartsWith(Token.TokenType.ATTR));
            Assert.True(" pRofilE. (abc".StartsWith(Token.TokenType.ATTR));
            Assert.False(" profile )  (abc".StartsWith(Token.TokenType.ATTR));
            Assert.False(" porfile )  (abc".StartsWith(Token.TokenType.ATTR));
        }
        
        [Fact]
        public void GetStringLiteralTest()
        {
            Assert.Equal("23isjdfj", "\"23isjdfj\"".GetStringLiteral());
            Assert.Equal("23isjdfj", "  \"23isjdfj\"".GetStringLiteral());
            Assert.Null( "sd \"23isjdfj\"".GetStringLiteral());
        }
        
        [Fact]
        public void GetNextTokenTest()
        {
            Assert.Equal(Token.TokenType.EQ, "eq 123".GetNextToken().Item1.Type);
            Assert.Equal(Token.TokenType.PR, " pr 123".GetNextToken().Item1.Type);
            Assert.Equal(Token.TokenType.NOT, " not 123".GetNextToken().Item1.Type);
            Assert.Equal(Token.TokenType.OPEN, " (not 123)".GetNextToken().Item1.Type);
            Assert.Equal(Token.TokenType.CLOSE, " )".GetNextToken().Item1.Type);
            Assert.Equal(Token.TokenType.STR, " \"adfsasdf 123\"".GetNextToken().Item1.Type);
            Assert.Equal(Token.TokenType.ATTR, " profile.LOA eq \"123\"".GetNextToken().Item1.Type);
            Assert.Equal(Token.TokenType.END, "".GetNextToken().Item1.Type);
        }

        [Fact]
        public void GetAttrTest()
        {
            Assert.Equal("abcd", "profile.abcd".GetAttrName());
            Assert.Equal("Abc1", "profile.Abc1".GetAttrName());
            Assert.Equal("Abc1", " profile.Abc1".GetAttrName());
        }
    }
}