using System;

namespace Sharpy.Lexer
{
    public struct UnboundToken : IEquatable<UnboundToken>
    {
        public string Value { get; set; }

        public UnboundToken(string value)
        {
            Value = value;
        }

        public bool Equals(UnboundToken rhs) => Value == rhs.Value;

        public override string ToString() => $"UnboundToken({Value})";
    }
}
