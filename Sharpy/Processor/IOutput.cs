using System.Collections.Generic;

namespace Sharpy.Processor
{
    public interface IOutput<TOutput>
    {
        void AddChild(TOutput child);

         void SetRuleName(string rule_name);
    }
}