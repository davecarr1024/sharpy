using Sharpy.Processor;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Lexer
{
    public class Lexer : Processor<State, IEnumerable<Token>>
    {
        private static Dictionary<string, Rule> WrapRules(Dictionary<string, RegExp> regexes)
        {
            if (regexes.Keys.Any(name => name.StartsWith("_")))
            {
                throw new Error("lexer rule names can't start with _");
            }
            var rules = new Dictionary<string, Rule> {
                { "_root", until_empty(or(regexes.Keys.Select(rule_name => ref_(rule_name)).ToArray())) }
            };
            foreach (var item in regexes)
            {
                rules[item.Key] = item.Value;
            }
            return rules;
        }

        public Lexer(Dictionary<string, RegExp> rules) : base(WrapRules(rules), "_root") { }

        public override State Advance(State input, IEnumerable<Token> output) => input.Advance(output);

        public override IEnumerable<Token> Aggregate(Context context, IEnumerable<IEnumerable<Token>> outputs)
            => outputs.Aggregate((tokens, output) => tokens.Concat(output));

        public override bool Empty(State input) => input.Empty();

        public override Location? Location(State input) => input.Location;

        public override IEnumerable<Token> SetRuleName(IEnumerable<Token> output, string rule_name)
            => rule_name.StartsWith("_") ? output : output.Select(token => token.WithRuleName(rule_name));

        public IEnumerable<Token> Apply(string input) => Apply(new State(input, 0, new Sharpy.Lexer.Location(0, 0)));
    }
}