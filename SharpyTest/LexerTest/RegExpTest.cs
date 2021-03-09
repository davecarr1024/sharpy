using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Lexer;
using Sharpy.Processor;
using SharpyTest.ProcessorTest;
using System.Collections.Generic;
using System.Linq;

namespace SharpyTest.LexerTest
{
    [TestClass]
    public class TerminalTest : RuleTest<string, string, RegExp.Error>
    {
        public override Processor<string, string> Processor() => new RegExp(null);

        [TestMethod]
        public void TestApply()
        {
            TestRule(new RegExp.Terminal('a'),
                new List<(string, string)>{
                    ("", null),
                    ("a", "a"),
                    ("b", null),
                });
        }
    }

    [TestClass]
    public class ClassTest : RuleTest<string, string, RegExp.Error>
    {
        public override Processor<string, string> Processor() => new RegExp(null);

        [TestMethod]
        public void TestApply()
        {
            TestRule(new RegExp.Class('a', 'z'),
                new List<(string, string)>{
                    ("", null),
                    ("a", "a"),
                    ("m", "m"),
                    ("z", "z"),
                    ("1", null),
                });
        }
    }

    [TestClass]
    public class NotTest : RuleTest<string, string, RegExp.Error>
    {
        public override Processor<string, string> Processor() => new RegExp(null);

        [TestMethod]
        public void TestApply()
        {
            TestRule(RegExp.not(RegExp.terminal('a')),
                new List<(string, string)>{
                    ("", null),
                    ("a", null),
                    ("ab", null),
                    ("b", "b"),
                    ("ba", "b"),
                });
        }
    }

    [TestClass]
    public class RegExpTest : RuleTest<State, IEnumerable<Token>, Lexer.Error>
    {
        public override Processor<State, IEnumerable<Token>> Processor()
            => new Sharpy.Lexer.Lexer(new Dictionary<string, RegExp>());

        public override void CheckOutput(IEnumerable<Token> expected, IEnumerable<Token> output)
            => CollectionAssert.AreEqual(expected.ToList(), output.ToList());

        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new RegExp(
                    RegExp.until_empty(
                        RegExp.and(
                            RegExp.or(
                                RegExp.terminal('a'),
                                RegExp.terminal('b')
                            ),
                            RegExp.one_or_more(
                                RegExp.class_('0', '9')
                            )
                        )
                    )
                ),
                new List<(State, IEnumerable<Token>)>
                {
                    (
                        new State("a1", 0, new Location(0,0)),
                        new List<Token>{new Token("", "a1", new Location(0,0))}
                    ),
                    (
                        new State("b1", 0, new Location(0,0)),
                        new List<Token>{new Token("", "b1", new Location(0,0))}
                    ),
                    (
                        new State("a123", 0, new Location(0,0)),
                        new List<Token>{new Token("", "a123", new Location(0,0))}
                    ),
                    (
                        new State("", 0, new Location(0,0)),
                        new List<Token>{new Token("", "", new Location(0,0))}
                    ),
                    (
                        new State("a", 0, new Location(0,0)),
                        null
                    ),
                    (
                        new State("c", 0, new Location(0,0)),
                        null
                    ),
                    (
                        new State("ac", 0, new Location(0,0)),
                        null
                    ),
                }
            );
        }

        [TestMethod]
        public void TestEqual()
        {
            Assert.AreEqual(new RegExp(null), new RegExp(null));
            Assert.AreEqual(new RegExp(RegExp.terminal('a')), new RegExp(RegExp.terminal('a')));
            Assert.AreNotEqual(new RegExp(RegExp.terminal('a')), new RegExp(RegExp.class_('0', '9')));
            Assert.AreEqual(
                new RegExp(RegExp.and(RegExp.and(RegExp.terminal('a'), RegExp.terminal('b'), RegExp.terminal('c')))),
                new RegExp(RegExp.and(RegExp.and(RegExp.terminal('a'), RegExp.terminal('b'), RegExp.terminal('c'))))
            );
        }

        [TestMethod]
        public void TestRead()
        {
            foreach ((var input, var expected) in new List<(string, RegExp)>{
                (
                    "abc",
                    new RegExp(
                        RegExp.and(
                            RegExp.terminal('a'),
                            RegExp.terminal('b'),
                            RegExp.terminal('c')
                        )
                    )
                ),
                (
                    "a*b",
                    new RegExp(
                        RegExp.and(
                            RegExp.zero_or_more(
                                RegExp.terminal('a')
                            ),
                            RegExp.terminal('b')
                        )
                    )
                ),
            })
            {
                Assert.AreEqual(expected, RegExp.Build(input));
            }
        }
    }
}

