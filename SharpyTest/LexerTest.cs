using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy;
using System.Collections.Generic;

namespace SharpyTest
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestLiteral()
        {
            Assert.AreEqual(
                new Lexer.UnboundToken("a"),
                new Lexer.Literal("a").Apply("a")
            );
            Assert.AreEqual(
                null,
                new Lexer.Literal("a").Apply("b")
            );
        }

        [TestMethod]
        public void TestApply()
        {
            CollectionAssert.AreEquivalent(
                new List<Lexer.Token>(){
                    new Lexer.Token("ar", "av", new Lexer.Location(0,0)),
                    new Lexer.Token("br", "bv", new Lexer.Location(0,2)),
                    new Lexer.Token("cr", "cv", new Lexer.Location(0,4)),
                    },
                new Lexer(
                    new Dictionary<string, Lexer.Rule>()
                    {
                        {"ar", new Lexer.Literal("av")},
                        {"br", new Lexer.Literal("bv")},
                        {"cr", new Lexer.Literal("cv")},
                    },
                    new Dictionary<string, Lexer.Rule>()
                    {

                    }).Apply("avbvcv")
            );
        }
    }
}
