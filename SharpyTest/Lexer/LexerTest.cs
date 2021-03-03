using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Lexer;
using System.Collections.Generic;

namespace SharpyTest.Lexer
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestApply()
        {
            CollectionAssert.AreEquivalent(
                new List<Token>(){
                    new Token("ar", "av", new Location(0,0)),
                    new Token("br", "bv", new Location(0,2)),
                    new Token("cr", "cv", new Location(0,4)),
                    },
                new Sharpy.Lexer.Lexer(
                    new Dictionary<string, Rule>()
                    {
                        {"ar", new Literal("av")},
                        { "br", new Literal("bv")},
                        { "cr", new Literal("cv")},
                    },
                    new Dictionary<string, Rule>()
                    {

                    }).Apply("avbvcv")
            );
        }
    }
}
