using System;

namespace Sharpy.Lexer
{
    public class Literal : Rule
    {
        public string Value { get; }

        public Literal(string val) => Value = val;

        public override bool Equals(object obj) => obj is Literal literal && Value == literal.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"Literal({Value})";

        public UnboundToken? Apply(string s) => s.StartsWith(Value) ? new UnboundToken(Value) : new UnboundToken?();
    }
}
