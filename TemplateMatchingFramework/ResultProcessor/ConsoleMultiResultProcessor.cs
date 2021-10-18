using System;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.ResultProcessor
{
    public class ConsoleMultiResultProcessor : MultiResultProcessor
    {
        public ConsoleMultiResultProcessor(params IMatchResultProcessor[] processors) : base(processors){}

        public override void OnBeforeProcess(IMatchInput input, IMatchResultProcessor processor, IMatchResult result)
        {
            Console.WriteLine($"[Multi][{processor.GetType().Name}] => {input}");
        }
    }
}