using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class ZeroOrMoreTest : IntFilterRuleTest
    {
        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new IntFilter.ZeroOrMore(new Equals(1)),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{}, new List<int>{}),
                    (new List<int>{1}, new List<int>{1}),
                    (new List<int>{1, 2}, new List<int>{1}),
                    (new List<int>{1, 1, 2}, new List<int>{1, 1}),
                }
            );
        }
    }
}