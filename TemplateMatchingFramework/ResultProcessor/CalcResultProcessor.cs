using System;
using System.IO;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.ResultProcessor
{
    public class CalcResultProcessor : IMatchResultProcessor
    {
        [Flags]
        public enum ResultType
        {
            Heat = 2,
            Mask = 4,
        }

        public ResultType Type = ResultType.Heat;

        public bool CanProcess(IMatchInput input, IMatchResult result) => result is CalcMatchResult;

        public void Process(IMatchInput input, IMatchResult result)
        {
            var calcResult = result as CalcMatchResult;
            if(calcResult == null)return;
            
            if (Type.HasFlag(ResultType.Heat))
            {
                var resultDir = Path.Combine(input.BasePath, "HeatResults");
                Directory.CreateDirectory(resultDir);
                var filename = Path.Combine(resultDir, $"{input.Name}_{input.SubName}.png");
                calcResult.PointCalculator.GetHeatmap().Save(filename);
            }

            if (Type.HasFlag(ResultType.Mask))
            {
                var resultDir = Path.Combine(input.BasePath, "MaskResults");
                Directory.CreateDirectory(resultDir);
                var filename = Path.Combine(resultDir, $"{input.Name}_{input.SubName}.png");
                calcResult.PointCalculator.GetMask().Save(filename);
            }
        }
    }
}