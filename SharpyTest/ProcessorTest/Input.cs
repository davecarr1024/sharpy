using Sharpy.Lexer;
using Sharpy.Processor;
using System.Collections.Generic;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    public class Input : IInput<Input, Output>
    {
        public IEnumerable<int> Vals { get; }

        public int Pos { get; }

        public Input(IEnumerable<int> vals, int pos = 0)
        {
            Vals = vals;
            Pos = pos;
        }

        public Input Advance(Output output)
        {
            int count = output.NumVals();
            return new Input(Vals.Skip(count), Pos + count);
        }

        public bool Empty() => !Vals.Any();

        public Location? Location() => new Location(0, Pos);
    }
}