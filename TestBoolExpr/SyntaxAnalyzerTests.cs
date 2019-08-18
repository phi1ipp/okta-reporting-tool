using reporting_tool;
using reporting_tool.BoolExpr;
using Xunit;

namespace TestBoolExpr
{
    public class SyntaxAnalyzerTests
    {
        [Fact]
        public void DictionaryNotEmpty()
        {
            Assert.NotEmpty(SyntaxAnalyzer.Dictionary);
        }

        [Fact]
        public void RuleMatchTest()
        {
            Assert.True(
                SyntaxAnalyzer.Match(
                    Tokenizer.Tokenize("profile.LOA eq \"3\"")));
        }
    }
}