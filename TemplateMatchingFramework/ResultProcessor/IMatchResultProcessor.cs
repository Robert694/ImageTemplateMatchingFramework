using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.ResultProcessor
{
    public interface IMatchResultProcessor
    {
        public bool CanProcess(IMatchInput input, IMatchResult result);
        public void Process(IMatchInput input, IMatchResult result);
    }
}