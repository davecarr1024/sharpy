using Sharpy.Processor;
using System.Collections.Generic;
using System.Linq;

namespace SharpyTest.ProcessorTest
{
    public class Output : IOutput<Output>
    {
        public IList<Output> Children { get; } = new List<Output>();

        public string RuleName { get; set; } = null;

        public int? Val { get; } = null;

        public Output() { }

        public Output(int val, string rule_name = "")
        {
            Val = val;
            RuleName = rule_name;
        }

        public Output(IList<Output> children) => Children = children;

        public void AddChild(Output child) => Children.Add(child);

        public void SetRuleName(string rule_name) => RuleName = rule_name;

        public override bool Equals(object obj)
            => obj is Output rhs && Val.Equals(rhs.Val) && RuleName == rhs.RuleName && Children.SequenceEqual(rhs.Children);

        public override int GetHashCode() => Val.GetHashCode();

        public override string ToString()
            => string.Format(
                "TestOutput(Val='{0}', RuleName='{1}', Children=[{2}])",
                Val, RuleName, string.Join(", ", Children.Select(child => child.ToString())));

        public int NumVals() => (Val != null ? 1 : 0) + Children.Sum(child => child.NumVals());
    }
}