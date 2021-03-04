using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Processor;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class RefTest : IntFilterRuleTest
    {
        public override Processor<int, IEnumerable<int>> Processor()
            => new IntFilter(new Dictionary<string, IntFilter.Rule>{
                    {"a", new Equals(1)}
                }, "");

        [TestMethod]
        public void TestApply()
        {
            TestRule(
                new IntFilter.Ref("a"),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{1}, new List<int>{1}),
                    (new List<int>{2}, null),
                }
            );
            TestRule(
                new IntFilter.Ref("b"),
                new List<(IEnumerable<int>, IEnumerable<int>)> {
                    (new List<int>{1}, null),
                }
            );
        }
    }
}