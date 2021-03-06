using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class OneOrMoreTest : IntFilterRuleTest
    {
        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new IntFilter.OneOrMore(new Equals(1)),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{}, null),
                    (new List<int>{1}, new List<int>{1}),
                    (new List<int>{1, 2}, new List<int>{1}),
                    (new List<int>{1, 1, 2}, new List<int>{1, 1}),
                }
            );
        }
    }
}