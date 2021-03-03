using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Lexer
{
    public class Lexer
    {

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
