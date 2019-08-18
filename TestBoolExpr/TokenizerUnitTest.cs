using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using reporting_tool;
using Xunit;

namespace TestBoolExpr
{
    public class TokenizerUnitTest
    {
        [Fact]
        public void TokenizeTest1()
        {
            var lstTokens = Tokenizer.Tokenize("profile.SourceType eq \"eidm\"").ToArray();
            Assert.NotEmpty(lstTokens);
            Assert.Equal(lstTokens[0]?.Type, Token.TokenType.ATTR);
            Assert.Equal(lstTokens[1]?.Type, Token.TokenType.EQ);
            Assert.Equal(lstTokens[2]?.Type, Token.TokenType.STR);
        }
        
        [Fact]
        public void TokenizeTest2()
        {
            var lstTokens = Tokenizer.Tokenize("profile.SourceType eq \"eidm\" and not profile.LOA pr").ToArray();
            Assert.NotEmpty(lstTokens);
            Assert.Equal(lstTokens[0]?.Type, Token.TokenType.ATTR);
            Assert.Equal(lstTokens[1]?.Type, Token.TokenType.EQ);
            Assert.Equal(lstTokens[2]?.Type, Token.TokenType.STR);
            Assert.Equal(lstTokens[3]?.Type, Token.TokenType.AND);
            Assert.Equal(lstTokens[4]?.Type, Token.TokenType.NOT);
            Assert.Equal(lstTokens[5]?.Type, Token.TokenType.ATTR);
            Assert.Equal(lstTokens[6]?.Type, Token.TokenType.PR);
        }
    }
}