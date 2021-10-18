using System;
using System.Diagnostics;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Match;
using TemplateMatchingFramework.Result;
using TemplateMatchingFramework.ResultProcessor;

namespace TemplateMatchingFramework
{
    public class ConsoleImageProcessor : ImageProcessor
    {
        public string Name { get; }
        public ConsoleImageProcessor(string path, string subImageFolder, IMatchProcessor bestMatchProcessor, IMatchResultProcessor resultProcessor) : base(path, subImageFolder, bestMatchProcessor, resultProcessor)
        {
            Name = BestMatchProcessor.GetType().Name;
        }

        private Stopwatch sw;

        public override void OnBeforeInput(IMatchInput input)
        {
            if (sw != null) Console.WriteLine();
            Console.WriteLine($"[{Name}] => {input}");
            sw = Stopwatch.StartNew();
        }

        public override void OnAfterInput(IMatchInput input, IMatchResult result)
        {
            sw.Stop();
            Console.WriteLine($"[{Name}] <= {input} [{sw.ElapsedMilliseconds} ms] [Score: {result.Score}] - {result.Point}");
        }
    }
}