using System.Collections.Generic;
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
                throw context.Error("no input");
            }
            if (context.Input.First() != Val)
            {
                throw context.Error($"failed to match {Val}");
            }
            return new List<int> { Val };
        }
    }
}