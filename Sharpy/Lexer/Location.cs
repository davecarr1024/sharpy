using System;

namespace Sharpy.Lexer
{
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
}
