using Sharpy.Lexer;
using Sharpy.Parser;
using Sharpy.Syntax;
using Sharpy.Processor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Lexer
{
    public class RegExp : Processor<string, string>, Lexer.Rule
    {
        public class Terminal : Rule
        {
            public char Value { get; }

            public Terminal(char value) => Value = value;

            public override bool Equals(object obj) => obj is Terminal rhs && Value == rhs.Value;

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"Terminal('{Value}')";

            public string Apply(Context context)
            {
                if (!context.Input.Any())
                {
                    throw context.Error($"no input for {this}");
                }
                char c = context.Input.First();
                if (c != Value)
                {
                    throw context.Error($"failed to match {this} in {context.Input}");
                }
                return c.ToString();
            }
        }

        public class Class : Rule
        {
            public char Min { get; }

            public char Max { get; }

            public Class(char min, char max)
            {
                Min = min;
                Max = max;
            }

            public override bool Equals(object obj) => obj is Class rhs && Min == rhs.Min && Max == rhs.Max;

            public override int GetHashCode() => HashCode.Combine(Min, Max);

            public override string ToString() => $"Class({Min},{Max})";

            public string Apply(Context context)
            {
                if (!context.Input.Any())
                {
                    throw context.Error($"no input for {this}");
                }
                char c = context.Input.First();
                if (c < Min || c > Max)
                {
                    throw context.Error($"failed to match {this} in {context.Input}");
                }
                return c.ToString();
            }
        }

        public class Not : Rule
        {
            public Rule Rule { get; }

            public Not(Rule rule) => Rule = rule;

            public override bool Equals(object obj) => obj is Not rhs && Rule.Equals(rhs.Rule);

            public override int GetHashCode() => Rule.GetHashCode();

            public override string ToString() => $"Not({Rule})";

            public string Apply(Context context)
            {
                if (!context.Input.Any())
                {
                    throw context.Error($"no input for {this}");
                }
                try
                {
                    Rule.Apply(context);
                }
                catch (Error)
                {
                    return context.Input.First().ToString();
                }
                throw context.Error($"failed to match {this} in {context.Input}");
            }
        }

        public static Terminal terminal(char c) => new Terminal(c);

        public static Class class_(char min, char max) => new Class(min, max);

        public static Not not(Rule rule) => new Not(rule);

        public RegExp(Rule rule) : base(new Dictionary<string, Rule> { { "root", rule } }, "root") { }

        public override string Advance(string input, string output) => input.Substring(output.Length);

        public override string Aggregate(Context context, IEnumerable<string> outputs) => string.Join("", outputs);

        public override bool Empty(string input) => !input.Any();

        public IEnumerable<Token> Apply(Processor<State, IEnumerable<Token>>.Context context)
        {
            try
            {
                return new List<Token>
                {
                    new Token("",
                    Apply(context.Input.Input.Substring(context.Input.Pos)), context.Input.Location)
                };
            }
            catch (Error error)
            {
                throw new Sharpy.Lexer.Lexer.Error(error.Message);
            }
        }

        private static Lexer BuildLexer()
        {
            var operators = new List<char> { '*' };
            var rules = new Dictionary<string, RegExp>
            {
                {"any", new RegExp(not(or(operators.Select(op => terminal(op)).ToArray())))},
            };
            foreach (var op in operators)
            {
                rules[op.ToString()] = new RegExp(terminal(op));
            }
            return new Lexer(rules);
        }

        private static Parser.Parser BuildParser()
        {
            return new Parser.Parser(
                new Dictionary<string, Parser.Parser.Rule>{
                    {
                        "root",
                        Parser.Parser.until_empty(
                            Parser.Parser.ref_("rule")
                        )
                    },
                    {
                        "rule",
                        Parser.Parser.or(
                            Parser.Parser.ref_("operand")
                        )
                    },
                    {
                        "operand",
                        Parser.Parser.or(
                            Parser.Parser.ref_("unary_operation"),
                            Parser.Parser.ref_("unary_operand")
                        )
                    },
                    {
                        "unary_operand",
                        Parser.Parser.or(
                            Parser.Parser.literal("any")
                        )
                    },
                    {
                        "unary_operation",
                        Parser.Parser.or(
                            Parser.Parser.ref_("zero_or_more")
                        )
                    },
                    {
                        "zero_or_more",
                        Parser.Parser.and(
                            Parser.Parser.ref_("unary_operand"),
                            Parser.Parser.literal("*")
                        )
                    },
                },
                "root"
            );
        }

        private static Syntax.Syntax<Rule> BuildSyntax()
        {
            return new Syntax.Syntax<Rule>(new List<Syntax<Rule>.Rule>
            {
                Syntax.Syntax<Rule>.rule_name_is("any",
                    Syntax.Syntax<Rule>.factory(
                        state=> new List<Rule>{
                            RegExp.terminal(state.Node.Token.Value.Value.First())
                        }
                    )
                ),
                Syntax.Syntax<Rule>.rule_name_is("zero_or_more",
                    Syntax.Syntax<Rule>.factory(
                        state=> new List<Rule>{
                            RegExp.zero_or_more(state.Exprs.First())
                        }
                    )
                ),
            });
        }

        public static RegExp Build(string input)
        {
            return new RegExp(
                RegExp.and(
                    BuildSyntax().Apply(
                        BuildParser().Apply(
                            BuildLexer().Apply(input)
                        )
                    ).ToArray()
                )
            );
        }
    }
}