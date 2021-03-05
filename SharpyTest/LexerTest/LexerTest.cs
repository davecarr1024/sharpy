using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Lexer;
using System.Collections.Generic;
using System.Linq;

namespace SharpyTest.LexerTest
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var input, var expected) in new List<(string, IEnumerable<Token>)>
            {
                ("a", new List<Token>{new Token("a_rule", "a", new Location(0,0))}),
                ("b", new List<Token>{new Token("b_rule", "b", new Location(0,0))}),
                (
                    "ab",
                    new List<Token>{
                        new Token("a_rule", "a", new Location(0,0)),
                        new Token("b_rule", "b", new Location(0,1)),
                    }
                ),
            })
            {
                CollectionAssert.AreEqual(
                    expected.ToList(),
                    new Lexer(new Dictionary<string, RegExp>
                    {
                        {"a_rule", new RegExp(RegExp.terminal('a'))},
                        {"b_rule", new RegExp(RegExp.terminal('b'))},
                    }).Apply(input).ToList()
                );
            }
        }
    }
}