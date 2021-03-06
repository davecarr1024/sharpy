using System.Linq;
using Sharpy.Parser;
using System;
using System.Collections.Generic;

namespace Sharpy.Syntax
{
    public struct State<TExpr>
    {
        public Node Node { get; }

        public IEnumerable<TExpr> Exprs { get; }

        public State(Node node, IEnumerable<TExpr> exprs)
        {
            Node = node;
            Exprs = exprs;
        }

        public override bool Equals(object obj) => obj is State<TExpr> rhs && Node.Equals(rhs.Node) && Exprs.SequenceEqual(rhs.Exprs);

        public override int GetHashCode() => HashCode.Combine(Node, Exprs);

        public override string ToString() => $"State({Node}, [{string.Join(", ", Exprs.Select(expr => expr.ToString()))}])";
    }
}