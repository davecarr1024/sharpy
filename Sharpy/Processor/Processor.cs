using System.Xml;
using Sharpy.Errors;
using System.Linq;
using System.Collections.Generic;

namespace Sharpy.Processor
{
    public class Processor<TInput, TOutput>
        where TInput : IInput<TInput, TOutput>
        where TOutput : IOutput<TOutput>, new()
    {
        public struct Context
        {
            public Processor<TInput, TOutput> Processor { get; }

            public TInput Input { get; }

            public Context(Processor<TInput, TOutput> processor, TInput input)
            {
                Processor = processor;
                Input = input;
            }

            public Context Advance(TOutput output)
            {
                return new Context(Processor, Input.Advance(output));
            }
        }

        public interface Rule
        {
            TOutput Apply(Context context);
        }

        public class And : Rule
        {
            public IEnumerable<Rule> Rules { get; }

            public And(IEnumerable<Rule> rules)
            {
                Rules = rules;
            }

            public override bool Equals(object obj) => obj is And rhs && Rules.SequenceEqual(rhs.Rules);

            public override int GetHashCode() => Rules.GetHashCode();

            public override string ToString() => string.Format("And({0})", string.Join(", ", Rules.Select(rule => rule.ToString())));

            public TOutput Apply(Context context)
            {
                TOutput output = new TOutput();
                foreach (var rule in Rules)
                {
                    output.AddChild(rule.Apply(context.Advance(output)));
                }
                return output;
            }
        }

        public class Or : Rule
        {
            public IEnumerable<Rule> Rules { get; }

            public Or(IEnumerable<Rule> rules)
            {
                Rules = rules;
            }

            public override bool Equals(object obj) => obj is Or rhs && Rules.SequenceEqual(rhs.Rules);

            public override int GetHashCode() => Rules.GetHashCode();

            public override string ToString() => string.Format("Or({0})", string.Join(", ", Rules.Select(rule => rule.ToString())));

            public TOutput Apply(Context context)
            {
                var errors = new List<Error>();
                var rule_outputs = new List<TOutput>();
                foreach (var rule in Rules)
                {
                    try
                    {
                        rule_outputs.Add(rule.Apply(context));
                    }
                    catch (Error error)
                    {
                        errors.Add(error);
                    }
                }
                if (rule_outputs.Count == 1)
                {
                    var output = new TOutput();
                    output.AddChild(rule_outputs.First());
                    return output;
                }
                else if (rule_outputs.Count > 1)
                {
                    throw new Error($"ambiguous or {rule_outputs}");
                }
                else
                {
                    throw new CompoundError(errors);
                }
            }
        }

        public class ZeroOrMore : Rule
        {
            public Rule Rule { get; }

            public ZeroOrMore(Rule rule) => Rule = rule;


            public override bool Equals(object obj) => obj is ZeroOrMore rhs && Rule.Equals(rhs.Rule);

            public override int GetHashCode() => Rule.GetHashCode();

            public override string ToString() => $"ZeroOrMore({Rule})";

            public TOutput Apply(Context context)
            {
                var output = new TOutput();
                while (true)
                {
                    try
                    {
                        output.AddChild(Rule.Apply(context.Advance(output)));
                    }
                    catch (Error)
                    {
                        return output;
                    }
                }
            }
        }

        public class OneOrMore : Rule
        {
            public Rule Rule { get; }

            public OneOrMore(Rule rule) => Rule = rule;


            public override bool Equals(object obj) => obj is OneOrMore rhs && Rule.Equals(rhs.Rule);

            public override int GetHashCode() => Rule.GetHashCode();

            public override string ToString() => $"OneOrMore({Rule})";

            public TOutput Apply(Context context)
            {
                var output = new TOutput();
                output.AddChild(Rule.Apply(context));
                while (true)
                {
                    try
                    {
                        output.AddChild(Rule.Apply(context.Advance(output)));
                    }
                    catch (Error)
                    {
                        return output;
                    }
                }
            }
        }

        public class ZeroOrOne : Rule
        {
            public Rule Rule { get; }

            public ZeroOrOne(Rule rule) => Rule = rule;


            public override bool Equals(object obj) => obj is ZeroOrOne rhs && Rule.Equals(rhs.Rule);

            public override int GetHashCode() => Rule.GetHashCode();

            public override string ToString() => $"ZeroOrOne({Rule})";

            public TOutput Apply(Context context)
            {
                var output = new TOutput();
                try
                {
                    output.AddChild(Rule.Apply(context));
                }
                catch (Error) { }
                return output;
            }
        }

        public class Ref : Rule
        {
            public string Val { get; }

            public Ref(string val) => Val = val;

            public override bool Equals(object obj) => obj is Ref rhs && Val.Equals(rhs.Val);

            public override int GetHashCode() => Val.GetHashCode();

            public override string ToString() => $"Ref({Val})";

            public TOutput Apply(Context context)
            {
                return context.Processor.ApplyRule(Val, context);
            }
        }

        public Dictionary<string, Rule> Rules { get; }

        public string Root { get; }

        public Processor(Dictionary<string, Rule> rules, string root)
        {
            Rules = rules;
            Root = root;
        }

        public TOutput ApplyRule(string rule_name, Context context)
        {
            if (!Rules.ContainsKey(rule_name))
            {
                throw new Errors.Error($"unknown rule '{rule_name}'");
            }
            TOutput output;
            try
            {
                output = Rules[rule_name].Apply(context);
            }
            catch (Errors.Error e)
            {
                throw new Errors.Error($"while applying rule '{rule_name}'", context.Input.Location(), e);
            }
            output.SetRuleName(rule_name);
            return output;
        }

        public TOutput Apply(TInput input)
        {
            return ApplyRule(Root, new Context(this, input));
        }
    }
}