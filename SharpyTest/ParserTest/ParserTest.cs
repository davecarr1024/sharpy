using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Lexer;
using Sharpy.Parser;
using System;
using System.Collections.Generic;

namespace SharpyTest.ParserTest
{
    [TestClass]
    public class LiteralTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var input, var expected) in new List<(IEnumerable<Token>, Node?)>{
                (
                    new List<Token>{new Token("a_rule", "a", new Location(0,1))},
                    new Node("", new Token("a_rule", "a", new Location(0,1)), new List<Node>{})
                ),
                (
                    new List<Token>{new Token("b_rule", "a", new Location(0,1))},
                    null
                ),
            })
            {
                Func<Node> apply = () => Parser.literal("a_rule").Apply(new Parser.Context(new Parser(null, null), input));
                if (expected is Node node)
                {
                    Assert.AreEqual(node, apply());
                }
                else
                {
                    Assert.ThrowsException<Parser.Error>(() => apply());
                }
            }
        }
    }

    [TestClass]
    public class ParserTest
    {
        public static IEnumerable<Token> tokens(params Token[] tokens) => new List<Token>(tokens);

        public static Token token(string rule_name, string val = null, Location? location = null)
            => new Token(rule_name, val != null ? val : rule_name, location is Location loc ? loc : new Location(0, 0));

        public static Node empty_node(params Node[] children) => new Node("", null, children);

        public static Node rule_node(string rule_name, params Node[] children) => new Node(rule_name, null, children);

        public static Node token_node(string rule_name, Token token) => new Node(rule_name, token, new List<Node>());

        [TestMethod]
        public void TestApply()
        {
            foreach ((var input, var expected) in new List<(IEnumerable<Token>, Node?)>
            {
                (
                    tokens(token("a")),
                    rule_node("root", empty_node(token_node("a_rule", token("a"))))
                ),
                // (
                //     tokens(token("b"), token("c")),
                //     rule_node("root", empty_node(
                //         rule_node("b_rule",
                //             token_node("", token("b")),
                //             token_node("", token("c")))))
                // ),
                // (
                //     tokens(token("b")),
                //     null
                // ),
            })
            {
                Func<Node> apply = () => new Parser(
                    new Dictionary<string, Parser.Rule>
                    {
                        {
                            "root",
                            Parser.until_empty(
                                Parser.or(
                                    Parser.ref_("a_rule"),
                                    Parser.ref_("b_rule")
                                )
                            )
                        },
                        {"a_rule", Parser.literal("a")},
                        {"b_rule", Parser.and(Parser.literal("b"), Parser.literal("c"))},
                    },
                    "root"
                ).Apply(input);
                if (expected is Node node)
                {
                    Assert.AreEqual(node, apply());
                }
                else
                {
                    Assert.ThrowsException<Parser.Error>(() => apply());
                }
            }
        }
    }
}