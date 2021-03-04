using Sharpy.Processor;
using System.Collections.Generic;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    public class IntFilter : Processor<IEnumerable<int>, IEnumerable<int>>
    {
        public IntFilter(Dictionary<string, Rule> rules, string root) : base(rules, root) { }

        public override IEnumerable<int> Advance(IEnumerable<int> input, IEnumerable<int> output)
        {
            return input.Skip(output.Count());
        }

        public override IEnumerable<int> Aggregate(Context context, IEnumerable<IEnumerable<int>> outputs)
        {
            var result = new List<int>();
            foreach (var output in outputs) {
                result.AddRange(output);
            }
            return result;
        }

        public override bool Empty(IEnumerable<int> input) => !input.Any();
    }
}