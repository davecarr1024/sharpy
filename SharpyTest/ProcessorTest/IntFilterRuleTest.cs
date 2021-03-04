using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Sharpy.Errors;
using Sharpy.Processor;
using System;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    public class IntFilterRuleTest<TError> : RuleTest<int, IEnumerable<int>, TError>
        where TError : Exception
    {
        public override Processor<int, IEnumerable<int>> Processor()
            => new IntFilter(new Dictionary<string, IntFilter.Rule>(), "");

        public override void CheckOutput(IEnumerable<int> expected, IEnumerable<int> output)
            => CollectionAssert.AreEqual(expected.ToList(), output.ToList());
    }

    public class IntFilterRuleTest : IntFilterRuleTest<Error> { }
}