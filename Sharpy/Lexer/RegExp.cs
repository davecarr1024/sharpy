using Sharpy.Processor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Lexer
{
    public class RegExp : Processor<string, string>, Lexer.Rule
    {
        public class Terminal : Rule
        {
            public char Value { get; }

            public Terminal(char value) => Value = value;

            public override bool Equals(object obj) => obj is Terminal rhs && Value == rhs.Value;

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"Terminal('{Value}')";

            public string Apply(Context context)
            {
                if (!context.Input.Any())
                {
                    throw new Error($"no input for {this}");
                }
                char c = context.Input.First();
                if (c != Value)
                {
                    throw new Error($"failed to match {this}");
                }
                return c.ToString();
            }
        }

        public class Class : Rule
        {
            public char Min { get; }

            public char Max { get; }

            public Class(char min, char max)
            {
                Min = min;
                Max = max;
            }

            public override bool Equals(object obj) => obj is Class rhs && Min == rhs.Min && Max == rhs.Max;

            public override int GetHashCode() => HashCode.Combine(Min, Max);

            public override string ToString() => $"Class({Min},{Max})";

            public string Apply(Context context)
            {
                if (!context.Input.Any())
                {
                    throw new Error($"no input for {this}");
                }
                char c = context.Input.First();
                if (c < Min || c > Max)
                {
                    throw new Error($"failed to match {this}");
                }
                return c.ToString();
            }
        }

        public static Terminal terminal(char c) => new Terminal(c);

        public static Class class_(char min, char max) => new Class(min, max);

        public RegExp(Rule rule) : base(new Dictionary<string, Rule> { { "root", rule } }, "root") { }

        public override string Advance(string input, string output) => input.Substring(output.Length);

        public override string Aggregate(Context context, IEnumerable<string> outputs) => string.Join("", outputs);

        public override bool Empty(string input) => !input.Any();

        public IEnumerable<Token> Apply(Processor<State, IEnumerable<Token>>.Context context) 
        {
            try 
            {
                return new List<Token> 
                { 
                    new Token("", 
                    Apply(context.Input.Input.Substring(context.Input.Pos)), context.Input.Location) 
                };
            } 
            catch (Error error) 
            {
                throw new Sharpy.Lexer.Lexer.Error(error.Message);
            }
        }
    }
}