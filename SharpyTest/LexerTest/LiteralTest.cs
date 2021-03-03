using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Lexer;
using System.Collections.Generic;

namespace SharpyTest.LexerTest
{
    [TestClass]
    public class LiteralTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var s, var expected) in new List<(string, UnboundToken?)>{
                ("a", new UnboundToken("a")),
                ("b", null),
            })
            {
                Assert.AreEqual(expected, new Literal("a").Apply(s));
            }
        }
    }
}