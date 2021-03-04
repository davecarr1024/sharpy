using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class ZeroOrOneTest : IntFilterRuleTest
    {
        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new IntFilter.ZeroOrOne(new Equals(1)),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{}, new List<int>{}),
                    (new List<int>{1}, new List<int>{1}),
                    (new List<int>{1, 2}, new List<int>{1}),
                }
            );
        }
    }
}