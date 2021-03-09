using Sharpy.Lexer;
using Sharpy.Processor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Syntax
{
    public class Syntax<TExpr> : Processor<State<TExpr>, IEnumerable<TExpr>>
    {
        public class RuleNameIs : Rule
        {
            public string RuleName { get; }

            public Rule Rule { get; }

            public RuleNameIs(string rule_name, Rule rule)
            {
                RuleName = rule_name;
                Rule = rule;
            }

            public override bool Equals(object obj) => obj is RuleNameIs rhs && Rule.Equals(rhs.Rule);

            public override int GetHashCode() => Rule.GetHashCode();

            public override string ToString() => $"RuleNameIs(RuleName={RuleName}, Rule={Rule})";

            public IEnumerable<TExpr> Apply(Context context)
            {
                if ((context.Input.Node.Token is Token tok && tok.RuleName == RuleName) || context.Input.Node.RuleName == RuleName)
                {
                    return Rule.Apply(context);
                }
                throw context.Error($"failed to match {this}");
            }
        }

        public class Factory : Rule
        {
            public Func<State<TExpr>, IEnumerable<TExpr>> Func { get; }

            public Factory(Func<State<TExpr>, IEnumerable<TExpr>> func) => Func = func;

            public IEnumerable<TExpr> Apply(Context context) => Func(context.Input);
        }

        public static RuleNameIs rule_name_is(string rule_name, Rule rule) => new RuleNameIs(rule_name, rule);

        public static Factory factory(Func<State<TExpr>, IEnumerable<TExpr>> func) => new Factory(func);

        public Syntax(IEnumerable<Rule> rules)
            : base(new Dictionary<string, Rule> { { "syntax_root", or(rules.ToArray()) } }, "syntax_root") { }

        public override State<TExpr> Advance(State<TExpr> input, IEnumerable<TExpr> output) => input;

        private static IEnumerable<TExpr> Aggregate(IEnumerable<IEnumerable<TExpr>> outputs)
            => !outputs.Any() ? new List<TExpr>() : outputs.Aggregate((result, output) => result.Concat(output));

        public override IEnumerable<TExpr> Aggregate(Context context, IEnumerable<IEnumerable<TExpr>> outputs)
            => Aggregate(outputs);

        public override bool Empty(State<TExpr> input) => true;

        public override Location? Location(State<TExpr> input) =>
            input.Node.Token is Token token ? token.Location : new Location?();

        public IEnumerable<TExpr> Apply(Parser.Node node)
        {
            var child_exprs = Aggregate(node.Children.Select(child => Apply(child)));
            try
            {
                return Apply(new State<TExpr>(node, child_exprs));
            }
            catch (Error)
            {
                return child_exprs;
            }
        }
    }
}