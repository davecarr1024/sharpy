using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Errors;
using System.Collections.Generic;

namespace SharpyTest.ErrorsTest
{
    [TestClass]
    public class ErrorTest
    {
        [TestMethod]
        public void TestEquals()
        {
            foreach ((var lhs, var rhs, bool expected) in new List<(Error, Error, bool)>
            {
                (new Error("a"),null,true),
                (new Error("a"),new Error("b"),false),
                (new Error("a", new Sharpy.Lexer.Location(0,1)),null,true),
                (new Error("a"), new Error("a", new Sharpy.Lexer.Location(0,1)),false),
                (new Error("a", new Sharpy.Lexer.Location(0,1)),new Error("a", new Sharpy.Lexer.Location(0,2)),false),
                (new Error("a", null, new Error("b")),null,true),
                (new Error("a", null, new Error("b")),new Error("a", null, new Error("c")),false),
            })
            {
                Assert.AreEqual(expected, lhs.Equals(rhs != null ? rhs : lhs), $"{lhs} == {rhs} ? {expected}");
            }
        }

    }
}
