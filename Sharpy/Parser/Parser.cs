using Sharpy.Errors;
using Sharpy.Lexer;
using Sharpy.Processor;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Parser
{
    public class Parser : Processor<IEnumerable<Token>, Node>
    {
        public class Literal : Rule
        {
            public string Value { get; }

            public Literal(string value) => Value = value;

            public override bool Equals(object obj) => obj is Literal rhs && Value == rhs.Value;

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"Literal({Value})";

            public Node Apply(Context context)
            {
                if (!context.Input.Any())
                {
                    throw new Error($"no input for {this}");
                }
                Token tok = context.Input.First();
                if (tok.RuleName != Value)
                {
                    throw new Error($"token {tok} failed to match {this}");
                }
                return new Node("", tok, new List<Node>());
            }
        }

        public static Literal literal(string value) => new Literal(value);

        public Parser(Dictionary<string, Rule> rules, string root) : base(rules, root) { }

        public override IEnumerable<Token> Advance(IEnumerable<Token> input, Node output) => input.Skip(output.NumTokens());

        public override Node Aggregate(Context context, IEnumerable<Node> outputs) => new Node("", null, outputs);

        public override bool Empty(IEnumerable<Token> input) => !input.Any();

        public override Location? Location(IEnumerable<Token> input) => input.Any() ? input.First().Location : new Location?();

        public override Node SetRuleName(Node output, string rule_name)
        {
            output.RuleName = rule_name;
            return output;
        }
    }
}