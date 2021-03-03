using System;

namespace Sharpy.Lexer
{
    public class Literal : Rule, IEquatable<Literal>
    {
        public string Value { get; }

        public Literal(string val) => Value = val;

        public bool Equals(Literal rhs) => Value == rhs.Value;

        public override string ToString() => $"Literal({Value})";

        public UnboundToken? Apply(string s) => s.StartsWith(Value) ? new UnboundToken(Value) : new UnboundToken?();
    }
}
