using Sharpy.Processor;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    public class Equals : Processor<Input, Output>.Rule
    {
        public int Val { get; }

        public Equals(int val)
        {
            Val = val;
        }

        public Output Apply(Processor<Input, Output>.Context context)
        {
            if (context.Input.Empty())
            {
                throw new Sharpy.Errors.Error("eof");
            }
            if (context.Input.Vals.First() != Val)
            {
                throw new Sharpy.Errors.Error($"unexpected {context.Input.Vals.First()} != {Val}", context.Input.Location());
            }
            return new Output(Val);
        }
    }
}