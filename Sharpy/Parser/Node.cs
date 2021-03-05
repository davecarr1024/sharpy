using Sharpy.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Parser
{
    public struct Node
    {
        public string RuleName { get; set; }

        public Token? Token { get; }

        public IEnumerable<Node> Children { get; }

        public Node(string rule_name, Token? token, IEnumerable<Node> children)
        {
            RuleName = rule_name;
            Token = token;
            Children = children;
        }

        public override bool Equals(object obj)
            => obj is Node rhs && RuleName == rhs.RuleName && Token.Equals(rhs.Token) && Children.SequenceEqual(rhs.Children);

        public override int GetHashCode() => HashCode.Combine(RuleName, Token, Children);

        public override string ToString()
            => $"Node(RuleName={RuleName}, Token={Token}, Children=[{string.Join(", ", Children.Select(child => child.ToString()))}]";

        public int NumTokens() => (Token != null ? 1 : 0) + Children.Sum(child=>child.NumTokens());
    }
}