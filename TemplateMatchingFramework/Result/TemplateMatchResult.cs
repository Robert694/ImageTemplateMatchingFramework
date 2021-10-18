using System.Drawing;

namespace TemplateMatchingFramework.Result
{
    public class TemplateMatchResult : IMatchResult
    {
        public TemplateMatchResult(Point point, float score, ModifiedAForgeExhaustiveTemplateMatching pointCalculator)
        {
            Point = point;
            Score = score;
            PointCalculator = pointCalculator;
        }

        public Point Point { get; }
        public float Score { get; }
        public ModifiedAForgeExhaustiveTemplateMatching PointCalculator { get; }
    }
}