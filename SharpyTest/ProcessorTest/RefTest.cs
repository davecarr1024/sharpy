using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Errors;
using Sharpy.Processor;
using System;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class RefTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var ref_name, var input, var expected) in new List<(string, Input, Output)>
            {
                ("a", new Input(new List<int>()), null),
                ("a", new Input(new List<int>{1}), new Output(1, "a")),
                ("a", new Input(new List<int>{1,2}), new Output(1, "a")),
                ("a", new Input(new List<int>{2}), null),
                ("b", new Input(new List<int>{1}), null),
            })
            {
                Func<Output> apply = () =>
                {
                    return new Processor<Input, Output>.Ref(
                            ref_name
                        ).Apply(new Processor<Input, Output>.Context(
                            new Processor<Input, Output>(
                                new Dictionary<string, Processor<Input, Output>.Rule>{
                                    {"a", new Equals(1)}
                                },
                                ""
                            ),
                            input));
                };
                if (expected != null)
                {
                    Assert.AreEqual(expected, apply(), input.ToString());
                }
                else
                {
                    Assert.ThrowsException<Error>(apply);
                }
            }
        }
    }
}