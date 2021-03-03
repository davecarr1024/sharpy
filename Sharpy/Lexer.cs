using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy
{
    public class Lexer
    {
        public class Error : Exception
        {
            public Location Location { get; }

            public Error(string message, Location location)
                : base($"{message} at {location}") 
                => Location = location;
        }

        public struct Location : IEquatable<Location>
        {
            public int Line { get; }
            public int Column { get; }

            public Location(int line, int column)
            {
                Line = line;
                Column = column;
            }

            public override string ToString() => $"Location(line={Line}, column={Column})";

            public bool Equals(Location rhs) => Line == rhs.Line && Column == rhs.Column;
        }

        public struct UnboundToken : IEquatable<UnboundToken>
        {
            public string Value { get; }

            public UnboundToken(string value)
            {
                Value = value;
            }

            public bool Equals(UnboundToken rhs) => Value == rhs.Value;

            public override string ToString() => $"UnboundToken({Value})";
        }

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

        public interface Rule
        {
            UnboundToken? Apply(string s);
        }

        public class Literal : Rule, IEquatable<Literal>
        {
            public string Value { get; }

            public Literal(string val) => Value = val;

            public bool Equals(Literal rhs) => Value == rhs.Value;

            public override string ToString() => $"Literal({Value})";

            public UnboundToken? Apply(string s) => s.StartsWith(Value) ? new UnboundToken(Value) : new UnboundToken?();
        }

        public Dictionary<string, Rule> Rules { get; }

        public Dictionary<string, Rule> SilentRules { get; }

        public Lexer(Dictionary<string, Rule> rules, Dictionary<string, Rule> silent_rules)
        {
            Rules = rules;
            SilentRules = silent_rules;
        }

        private IEnumerable<Token> ApplyRules(Dictionary<string, Rule> rules, string s, Location location)
        {
            return rules
                .Select(kv => new KeyValuePair<string, UnboundToken?>(kv.Key, kv.Value.Apply(s)))
                .Where(kv => kv.Value != null)
                .Select(kv => new Token(kv.Key, kv.Value.Value.Value, location));
        }

        public List<Token> Apply(string s)
        {
            var tokens = new List<Token>();
            int pos = 0, line = 0, column = 0;
            while (pos < s.Length)
            {
                var location = new Location(line, column);
                var rule_tokens = ApplyRules(Rules, s.Substring(pos), location);
                var silent_rule_tokens = ApplyRules(SilentRules, s.Substring(pos), location);
                if (!rule_tokens.Any() && !silent_rule_tokens.Any())
                {
                    throw new Error("unknown lex error", location);
                }
                else if (rule_tokens.Count() + silent_rule_tokens.Count() > 1)
                {
                    throw new Error($"ambiguous lex {rule_tokens} {silent_rule_tokens}", location);
                }
                Token token;
                if (rule_tokens.Any())
                {
                    token = rule_tokens.First();
                    tokens.Add(token);
                }
                else
                {
                    token = silent_rule_tokens.First();
                }
                foreach (var c in token.Value)
                {
                    if (c == '\n')
                    {
                        line++;
                        column = 0;
                    }
                    else
                    {
                        column++;
                        line = 0;
                    }
                    pos++;
                }
            }
            return tokens;
        }
    }
}
