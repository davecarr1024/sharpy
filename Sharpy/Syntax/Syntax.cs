using System.ComponentModel.DataAnnotations.Schema;
using Sharpy.Lexer;
using Sharpy.Processor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Syntax
{
    public class Syntax<TExpr> : Processor<State<TExpr>, IEnumerable<TExpr>>
    {
        public class Factory : Rule
        {
            public Func<State<TExpr>, IEnumerable<TExpr>> Func { get; }

            public Factory(Func<State<TExpr>, IEnumerable<TExpr>> func) => Func = func;

            public IEnumerable<TExpr> Apply(Context context) => Func(context.Input);
        }

        public static Factory factory(Func<State<TExpr>, IEnumerable<TExpr>> func) => new Factory(func);

        public Syntax(IEnumerable<Rule> rules) : base(new Dictionary<string, Rule> { { "root", or(rules.ToArray()) } }, "root") { }

        public override State<TExpr> Advance(State<TExpr> input, IEnumerable<TExpr> output) => input;

        public override IEnumerable<TExpr> Aggregate(Context context, IEnumerable<IEnumerable<TExpr>> outputs)
            => outputs.Aggregate((result, output) => result.Concat(output));

        public override bool Empty(State<TExpr> input) => true;

        public override Location? Location(State<TExpr> input) =>
            input.Node.Token is Token token ? token.Location : new Location?();
    }
}