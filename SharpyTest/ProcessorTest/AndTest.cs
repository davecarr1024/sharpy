using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class AndTest : IntFilterRuleTest
    {
        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new IntFilter.And(new List<IntFilter.Rule> { new Equals(1), new Equals(2) }),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{}, null),
                    (new List<int>{1}, null),
                    (new List<int>{1, 2}, new List<int>{1,2}),
                    (new List<int>{1, 2, 3}, new List<int>{1,2}),
                }
            );
        }
    }
}