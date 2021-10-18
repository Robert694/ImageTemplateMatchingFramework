using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.Match
{
    public interface IMatchProcessor
    {
        public IMatchResult Process(IMatchInput input);
    }
}