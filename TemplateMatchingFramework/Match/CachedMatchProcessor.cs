using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.Match
{
    public class CachedMatchProcessor : IMatchProcessor
    {
        public IMatchProcessor Processor { get; }
        private IMatchInput LastInput;
        private IMatchResult LastResult;
        public CachedMatchProcessor(IMatchProcessor processor)
        {
            Processor = processor;
        }

        public IMatchResult Process(IMatchInput input)
        {
            if (LastInput != null && input.BaseImage == LastInput.BaseImage)
            {
                LastInput = input;
                return LastResult;
            }
            LastInput = input;
            var result = Processor.Process(input);
            LastResult = result;
            return result;
        }
    }
}