using System.Drawing;

namespace TemplateMatchingFramework.Result
{
    public class MatchResult : IMatchResult
    {
        public Point Point { get; }
        public float Score { get; }

        public MatchResult(Point point, float score)
        {
            Point = point;
            Score = score;
        }
    }
}