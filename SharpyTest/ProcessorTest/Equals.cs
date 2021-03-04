using System.Collections.Generic;
using Sharpy.Errors;
using Sharpy.Processor;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    public class Equals : IntFilter.Rule
    {
        public int Val { get; }

        public Equals(int val) => Val = val;

        public IEnumerable<int> Apply(IntFilter.Context context)
        {
            if (!context.Input.Any())
            {
                throw new Error("no input");
            }
            if (context.Input.First() != Val)
            {
                throw new Error($"input {context.Input.First()} != expected {Val}");
            }
            return new List<int> { Val };
        }
    }
}