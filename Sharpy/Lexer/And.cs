using System.Linq;
using System;
using System.Collections.Generic;

namespace Sharpy.Lexer
{
    public class And : Rule
    {
        public List<Rule> Rules { get; }

        public And(List<Rule> rules) => Rules = rules;

        public override string ToString() => $"And({Rules})";

        public override bool Equals(object obj) => obj is And and && Rules.SequenceEqual(and.Rules);

        public override int GetHashCode() => Rules.GetHashCode();

        public UnboundToken? Apply(string s)
        {
            UnboundToken result = new UnboundToken("");
            foreach (var rule in Rules)
            {
                var rule_token = rule.Apply(s.Substring(result.Value.Length));
                if (rule_token == null)
                {
                    return null;
                }
                result.Value += rule_token.Value.Value;
            }
            return result;
        }
    }
}