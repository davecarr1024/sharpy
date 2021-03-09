using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Sharpy.Processor;
using System;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    public class IntFilterRuleTest : RuleTest<IEnumerable<int>, IEnumerable<int>, IntFilter.Error>
    {
        public override Processor<IEnumerable<int>, IEnumerable<int>> Processor()
            => new IntFilter(new Dictionary<string, IntFilter.Rule>(), "");

        public override void CheckOutput(IEnumerable<int> expected, IEnumerable<int> output)
            => CollectionAssert.AreEqual(expected.ToList(), output.ToList());
    }
}