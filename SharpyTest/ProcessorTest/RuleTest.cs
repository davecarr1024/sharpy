using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Processor;
using System.Collections.Generic;

namespace SharpyTest.ProcessorTest
{
    public abstract class RuleTest<TInput, TOutput, TError> where TError : Exception
    {
        public abstract Processor<TInput, TOutput> Processor();

        public virtual Processor<TInput, TOutput>.Context Context(TInput input)
            => new Processor<TInput, TOutput>.Context(Processor(), input);

        public virtual void CheckOutput(TOutput expected, TOutput output)
            => Assert.AreEqual(expected, output);

        public void TestRule(Processor<TInput, TOutput>.Rule rule, IEnumerable<(TInput, TOutput)> cases)
        {
            foreach ((var input, var output) in cases)
            {
                if (output != null)
                {
                    CheckOutput(output, rule.Apply(Context(input)));
                }
                else
                {
                    Assert.ThrowsException<TError>(() => rule.Apply(Context(input)));
                }
            }
        }
    }
}