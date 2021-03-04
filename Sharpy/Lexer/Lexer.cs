using Sharpy.Processor;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Lexer
{
    public class Lexer : Processor<State, IEnumerable<Token>>
    {
        public Lexer(Dictionary<string, Rule> rules, string root) : base(rules, root) { }

        public override State Advance(State input, IEnumerable<Token> output) => input.Advance(output);

        public override IEnumerable<Token> Aggregate(Context context, IEnumerable<IEnumerable<Token>> outputs)
            => outputs.Aggregate((tokens, output) => tokens.Concat(output));

        public override bool Empty(State input) => input.Empty();

        public override Location? Location(State input) => input.Location;
    }
}