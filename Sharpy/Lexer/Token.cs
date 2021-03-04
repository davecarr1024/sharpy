using System;

namespace Sharpy.Lexer
{
    public struct Token
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

        public Token WithRuleName(string rule_name) => new Token(rule_name, Value, Location);

        public override bool Equals(object obj)
            => obj is Token rhs && RuleName == rhs.RuleName && Value == rhs.Value && Location.Equals(rhs.Location);

        public override int GetHashCode() => HashCode.Combine(RuleName, Value, Location);

        public override string ToString() => $"Token(rule_name={RuleName}, value={Value}, location={Location})";
    }
}
