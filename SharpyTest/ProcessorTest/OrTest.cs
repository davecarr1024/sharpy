using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class OrTest : IntFilterRuleTest
    {
        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new IntFilter.Or(new List<IntFilter.Rule> { new Equals(1), new Equals(2) }),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{}, null),
                    (new List<int>{1}, new List<int>{1}),
                    (new List<int>{2}, new List<int>{2}),
                    (new List<int>{1, 2}, new List<int>{1}),
                    (new List<int>{3}, null),
                }
            );
        }
    }
}