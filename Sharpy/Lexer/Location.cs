using System;
using System.Collections.Generic;

namespace Sharpy.Lexer
{
    public struct Location : IComparable<Location>
    {
        public int Line { get; }
        public int Column { get; }

        public Location(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override int GetHashCode() => System.HashCode.Combine(Line, Column);

        public override string ToString() => $"Location(line={Line}, column={Column})";

        public override bool Equals(object obj) => obj is Location rhs && Line == rhs.Line && Column == rhs.Column;

        public int CompareTo(Location other) => Line == other.Line ? Column - other.Column : Line - other.Line;

        public Location Advance(IEnumerable<Token> tokens)
        {
            int line = Line;
            int column = Column;
            foreach (Token token in tokens)
            {
                foreach (char c in token.Value)
                {
                    if (c == '\n')
                    {
                        line++;
                        column = 0;
                    }
                    else
                    {
                        column++;
                    }
                }
            }
            return new Location(line, column);
        }

        public Location Advance(Token token) => Advance(new List<Token>{token});
    }
}
