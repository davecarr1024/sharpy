using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class IntFilterTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var input, var expected) in new List<(IEnumerable<int>, IEnumerable<int>)>
            {
                (new List<int>{1,2}, new List<int>{1,2}),
                (new List<int>{3}, new List<int>{3}),
                (new List<int>{1,2,3,1,2}, new List<int>{1,2,3,1,2}),
                (new List<int>{1,4}, null),
                (new List<int>{4}, null),
                (new List<int>{1,2,4}, null),
                (new List<int>{}, new List<int>{}),
            })
            {
                Func<IEnumerable<int>> apply = () =>
                {
                    return new IntFilter(new Dictionary<string, IntFilter.Rule>
                    {
                        {"a", new IntFilter.UntilEmpty(new IntFilter.Ref("b"))},
                        {"b", new IntFilter.Or(new List<IntFilter.Rule>{new IntFilter.Ref("c"), new IntFilter.Ref("d")})},
                        {"c", new IntFilter.And(new List<IntFilter.Rule>{new Equals(1), new Equals(2)})},
                        {"d", new Equals(3)},
                    }, "a").Apply(input);
                };
                if (expected != null)
                {
                    CollectionAssert.AreEqual(
                        expected.ToList(),
                        apply().ToList()
                    );
                }
                else
                {
                    Assert.ThrowsException<Error>(apply);
                }
            }
        }
    }
}
