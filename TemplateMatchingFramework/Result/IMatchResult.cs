using System.Drawing;

namespace TemplateMatchingFramework.Result
{
    public interface IMatchResult
    {
        public Point Point { get; }
        public float Score { get; }
    }
}