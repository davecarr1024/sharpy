using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpy.Errors;
using System.Collections.Generic;

namespace SharpyTest.Errors
{
    [TestClass]
    public class CompoundErrorTest
    {
        [TestMethod]
        public void TestAggregate()
        {
            foreach ((var errors, var expected_errors, var expected_location) in 
                new List<(IEnumerable<Error>, IEnumerable<Error>, Sharpy.Lexer.Location?)>{
                (new List<Error>(), new List<Error>(), null),
                (
                    new List<Error>{new Error("a")}, 
                    new List<Error>{new Error("a")}, 
                    null
                ),
                (
                    new List<Error>{new Error("a"),new Error("b")}, 
                    new List<Error>{new Error("a"),new Error("b")}, 
                    null
                ),
                (
                    new List<Error>{new Error("a", new Sharpy.Lexer.Location(0,1))}, 
                    new List<Error>{new Error("a", new Sharpy.Lexer.Location(0,1))}, 
                    new Sharpy.Lexer.Location(0,1)
                ),
                (
                    new List<Error>{
                        new Error("a", new Sharpy.Lexer.Location(0,1)),
                        new Error("b", new Sharpy.Lexer.Location(1,0)),
                    }, 
                    new List<Error>{new Error("b", new Sharpy.Lexer.Location(1,0))}, 
                    new Sharpy.Lexer.Location(1,0)
                ),
            })
            {
                var ce = new CompoundError(errors);
                CollectionAssert.AreEquivalent(expected_errors.ToList(), ce.Errors.ToList());
                Assert.AreEqual(expected_location, ce.Location);
            }
        }
    }
}