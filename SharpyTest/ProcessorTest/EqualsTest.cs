using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class EqualsTest : IntFilterRuleTest
    {
        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new Equals(1),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{1}, new List<int>{1}),
                    (new List<int>{1, 2}, new List<int>{1}),
                    (new List<int>{}, null),
                    (new List<int>{2}, null),
                }
            );
        }
    }
}