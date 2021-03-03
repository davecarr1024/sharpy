using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Lexer;
using System.Collections.Generic;

namespace SharpyTest.LexerTest
{
    [TestClass]
    public class AndTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var s, var expected) in new List<(string, UnboundToken?)>{
                ("ab", new UnboundToken("ab")),
                ("a", null),
                ("b", null),
            })
            {
                Assert.AreEqual(
                    expected,
                    new And(new List<Rule> {
                        new Literal("a"),
                        new Literal("b") })
                        .Apply(s));
            }
        }
    }
}