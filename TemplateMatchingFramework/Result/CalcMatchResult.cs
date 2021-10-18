using System.Drawing;

namespace TemplateMatchingFramework.Result
{
    public class CalcMatchResult : IMatchResult
    {
        public CalcMatchResult(Point point, float score, BestPointCalculator pointCalculator)
        {
            Point = point;
            Score = score;
            PointCalculator = pointCalculator;
        }

        public Point Point { get; }
        public float Score { get; }
        public BestPointCalculator PointCalculator { get; }
    }
}