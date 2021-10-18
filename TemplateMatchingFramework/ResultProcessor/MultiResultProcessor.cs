using System.Collections.Generic;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.ResultProcessor
{
    public class MultiResultProcessor : IMatchResultProcessor
    {
        public readonly List<IMatchResultProcessor> Processors = new();

        public MultiResultProcessor()
        {
        }

        public MultiResultProcessor(params IMatchResultProcessor[] processors)
        {
            Processors.AddRange(processors);
        }

        public bool CanProcess(IMatchInput input, IMatchResult result) => true;

        public void Process(IMatchInput input, IMatchResult result)
        {
            foreach (var p in Processors)
            {
                if(!p.CanProcess(input, result))continue;
                OnBeforeProcess(input, p, result);
                p.Process(input, result);
                OnAfterProcess(input, p, result);
            }
        }

        public virtual void OnBeforeProcess(IMatchInput input, IMatchResultProcessor processor, IMatchResult result){}
        public virtual void OnAfterProcess(IMatchInput input, IMatchResultProcessor processor, IMatchResult result){}
    }
}