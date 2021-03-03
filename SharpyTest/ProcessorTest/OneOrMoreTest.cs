using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Errors;
using Sharpy.Processor;
using System;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class OneOrMoreTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var input, var expected) in new List<(Input, Output)>
            {
                (new Input(new List<int>()), null),
                (new Input(new List<int>{1}), new Output(new List<Output>{new Output(1)})),
                (new Input(new List<int>{1,2}), new Output(new List<Output>{new Output(1)})),
                (new Input(new List<int>{1,1}), new Output(new List<Output>{new Output(1),new Output(1)})),
                (new Input(new List<int>{1,1,2}), new Output(new List<Output>{new Output(1),new Output(1)})),
            })
            {
                Func<Output> apply = () =>
                {
                    return new Processor<Input, Output>.OneOrMore(
                            new Equals(1)
                        ).Apply(new Processor<Input, Output>.Context(null, input));
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