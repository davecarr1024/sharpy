using System;

namespace Sharpy.Lexer
{
    public struct UnboundToken
    {
        public string Value { get; set; }

        public UnboundToken(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj) => obj is UnboundToken rhs && Value == rhs.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"UnboundToken({Value})";
    }
}
