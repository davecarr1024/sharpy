using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Errors;
using Sharpy.Processor;
using System;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    [TestClass]
    public class AndTest
    {
        [TestMethod]
        public void TestApply()
        {
            foreach ((var input, var expected) in new List<(Input, Output)>
            {
                (new Input(new List<int>()), null),
                (new Input(new List<int>{1}), null),
                (new Input(new List<int>{1,2}), new Output(new List<Output>{new Output(1), new Output(2)})),
                (new Input(new List<int>{1,2,3}), new Output(new List<Output>{new Output(1), new Output(2)})),
            })
            {
                Func<Output> apply = () =>
                {
                    return new Processor<Input, Output>.And(
                        new List<Processor<Input, Output>.Rule>{
                            new Equals(1),
                            new Equals(2),
                        }).Apply(new Processor<Input, Output>.Context(null, input));
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