using System;

namespace Sharpy.Lexer
{
    public struct Token : IEquatable<Token>
    {
        public string RuleName { get; }

        public string Value { get; }

        public Location Location { get; }

        public Token(string rule_name, string value, Location location)
        {
            RuleName = rule_name;
            Value = value;
            Location = location;
        }

        public bool Equals(Token rhs) => RuleName == rhs.RuleName && Value == rhs.Value && Location.Equals(rhs.Location);

        public override string ToString() => $"Token(rule_name={RuleName}, value={Value}, location={Location})";
    }
}
