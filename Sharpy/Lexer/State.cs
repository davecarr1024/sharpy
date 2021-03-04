using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Lexer
{
    public struct State
    {
        public string Input { get; }

        public int Pos { get; }

        public Location Location { get; }

        public State(string input, int pos, Location location)
        {
            Input = input;
            Pos = pos;
            Location = location;
        }

        public override bool Equals(object obj)
            => obj is State rhs && Input == rhs.Input && Pos == rhs.Pos && Location.Equals(rhs.Location);

        public override int GetHashCode() => HashCode.Combine(Input, Pos, Location);

        public override string ToString() => $"State(input={Input}, Pos={Pos}, Location={Location})";

        public State Advance(Token token) => Advance(new List<Token> { token });

        public State Advance(IEnumerable<Token> tokens) =>
            new State(Input, Pos + tokens.Sum(token => token.Value.Length), Location.Advance(tokens));

        public bool Empty() => Pos >= Input.Length;
    }
}