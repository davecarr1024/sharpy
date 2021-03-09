using Sharpy.Lexer;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sharpy.Processor
{
    public abstract class Processor<TInput, TOutput>
    {
        public class Error : Exception
        {
            public override string Message { get; }

            public Location? Location { get; }

            public Error(string message, Location? location = null)
            {
                Message = message;
                Location = location;
            }

            public Error(string message, Error error, Location? location = null)
                : this($"{message}: {error.Message}", MaxLocation(location, error.Location))
            { }

            public Error(IEnumerable<Error> errors)
            : this(AggregateMessages(errors.Where(error => error.Location.Equals(MaxLocation(errors.Select(e => e.Location).ToArray())))),
                    MaxLocation(errors.Select(error => error.Location).ToArray()))
            { }

            public override bool Equals(object obj) => obj is Error rhs && Message == rhs.Message && Location.Equals(rhs.Location);

            public override int GetHashCode() => HashCode.Combine(Message, Location);

            public override string ToString() => Location is Location loc ? $"{Message} at {loc}" : Message;

            private static Location? MaxLocation(params Location?[] locations)
            {
                var locs = locations.Where(location => location != null);
                return locs.Any() ? locs.Max() : null;
            }

            private static string AggregateMessages(IEnumerable<Error> errors)
            {
                if (errors.Count() > 1)
                {
                    return string.Format("[{0}]", string.Join(", ", errors.Select(error => error.Message)));
                }
                else if (errors.Count() == 1)
                {
                    return errors.First().Message;
                }
                else
                {
                    return "unknown error";
                }
            }

        }

        public struct Context
        {
            public Processor<TInput, TOutput> Processor { get; }

            public TInput Input { get; }

            public Location? Location { get { return Processor.Location(Input); } }

            public Context(Processor<TInput, TOutput> processor, TInput input)
            {
                Processor = processor;
                Input = input;
            }

            public Context Advance(TOutput output) => new Context(Processor, Processor.Advance(Input, output));

            public TOutput Aggregate(IEnumerable<TOutput> outputs) => Processor.Aggregate(this, outputs);

            public Error Error(string message) => new Error(message, Location);

            public Error Error(string message, Error error) => new Error(message, error, Location);
            public Error Error(IEnumerable<Error> errors) => new Error(errors);
        }

        public interface Rule
        {
            TOutput Apply(Context context);
        }

        public class And : Rule
        {
            public IEnumerable<Rule> Rules { get; }

            public And(params Rule[] rules) : this(rules.AsEnumerable()) { }

            public And(IEnumerable<Rule> rules)
            {
                Rules = rules;
            }

            public override bool Equals(object obj) => obj is And rhs && Rules.SequenceEqual(rhs.Rules);

            public override int GetHashCode() => Rules.GetHashCode();

            public override string ToString() => string.Format("And({0})", string.Join(", ", Rules.Select(rule => rule.ToString())));

            public TOutput Apply(Context context)
            {
                var outputs = new List<TOutput>();
                foreach (var rule in Rules)
                {
                    var output = rule.Apply(context);
                    outputs.Add(output);
                    context = context.Advance(output);
                }
                return context.Processor.Aggregate(context, outputs);
            }
        }

        public class Or : Rule
        {
            public IEnumerable<Rule> Rules { get; }

            public Or(params Rule[] rules) : this(rules.AsEnumerable()) { }

            public Or(IEnumerable<Rule> rules) => Rules = rules;

            public override bool Equals(object obj) => obj is Or rhs && Rules.SequenceEqual(rhs.Rules);

            public override int GetHashCode() => Rules.GetHashCode();

            public override string ToString() => string.Format("Or({0})", string.Join(", ", Rules.Select(rule => rule.ToString())));

            public TOutput Apply(Context context)
            {
                var errors = new List<Error>();
                foreach (var rule in Rules)
                {
                    try
                    {
                        return context.Aggregate(new List<TOutput> { rule.Apply(context) });
                    }
                    catch (Error error)
                    {
                        errors.Add(error);
                    }
                }
                throw context.Error(errors);
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
                var outputs = new List<TOutput>();
                while (true)
                {
                    try
                    {
                        var output = Rule.Apply(context);
                        outputs.Add(output);
                        context = context.Advance(output);
                    }
                    catch (Error)
                    {
                        return context.Processor.Aggregate(context, outputs);
                    }
                }
            }
        }

        public class UntilEmpty : Rule
        {
            public Rule Rule { get; }

            public UntilEmpty(Rule rule) => Rule = rule;


            public override bool Equals(object obj) => obj is UntilEmpty rhs && Rule.Equals(rhs.Rule);

            public override int GetHashCode() => Rule.GetHashCode();

            public override string ToString() => $"UntilEmpty({Rule})";

            public TOutput Apply(Context context)
            {
                var outputs = new List<TOutput>();
                while (!context.Processor.Empty(context.Input))
                {
                    var output = Rule.Apply(context);
                    outputs.Add(output);
                    context = context.Advance(output);
                }
                return context.Processor.Aggregate(context, outputs);
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
                var output = Rule.Apply(context);
                var outputs = new List<TOutput> { output };
                context = context.Advance(output);
                while (true)
                {
                    try
                    {
                        output = Rule.Apply(context);
                        outputs.Add(output);
                        context = context.Advance(output);
                    }
                    catch (Error)
                    {
                        return context.Processor.Aggregate(context, outputs);
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
                try
                {
                    var rule_output = Rule.Apply(context);
                    return context.Processor.Aggregate(
                        context.Advance(rule_output),
                        new List<TOutput> { rule_output });
                }
                catch (Error)
                {
                    return context.Processor.Aggregate(context, new List<TOutput>());
                }
            }
        }

        public class Ref : Rule
        {
            public string Val { get; }

            public Ref(string val) => Val = val;

            public override bool Equals(object obj) => obj is Ref rhs && Val.Equals(rhs.Val);

            public override int GetHashCode() => Val.GetHashCode();

            public override string ToString() => $"Ref({Val})";

            public TOutput Apply(Context context) => context.Processor.ApplyRule(Val, context);
        }

        public static And and(params Rule[] rules) => new And(rules.AsEnumerable());

        public static Or or(params Rule[] rules) => new Or(rules.AsEnumerable());

        public static ZeroOrMore zero_or_more(Rule rule) => new ZeroOrMore(rule);

        public static UntilEmpty until_empty(Rule rule) => new UntilEmpty(rule);

        public static OneOrMore one_or_more(Rule rule) => new OneOrMore(rule);

        public static ZeroOrOne zero_or_one(Rule rule) => new ZeroOrOne(rule);

        public static Ref ref_(string val) => new Ref(val);

        public Dictionary<string, Rule> Rules { get; }

        public string Root { get; }

        public Processor(Dictionary<string, Rule> rules, string root)
        {
            Rules = rules;
            Root = root;
        }

        public override bool Equals(object obj)
            => obj is Processor<TInput, TOutput> rhs && Rules.SequenceEqual(rhs.Rules) && Root == rhs.Root;

        public override int GetHashCode() => HashCode.Combine(Rules, Root);

        public override string ToString()
            => string.Format("Processor(Rules={0}, Root={1})",
                string.Join(", ", Rules.Select(i => i.ToString())),
                Root);

        public abstract TInput Advance(TInput input, TOutput output);

        public abstract TOutput Aggregate(Context context, IEnumerable<TOutput> outputs);

        public abstract bool Empty(TInput input);

        public virtual TOutput SetRuleName(TOutput output, string rule_name) => output;

        public virtual Location? Location(TInput input) => null;

        public TOutput ApplyRule(string rule_name, Context context)
        {
            if (!Rules.ContainsKey(rule_name))
            {
                throw context.Error($"unknown rule '{rule_name}'");
            }
            try
            {
                return SetRuleName(Rules[rule_name].Apply(context), rule_name);
            }
            catch (Error e)
            {
                throw context.Error($"while applying rule '{rule_name}'", e);
            }
        }

        public TOutput Apply(TInput input) => ApplyRule(Root, new Context(this, input));
    }
}