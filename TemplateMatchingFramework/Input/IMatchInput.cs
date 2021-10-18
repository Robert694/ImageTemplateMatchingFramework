using System.Drawing;

namespace TemplateMatchingFramework.Input
{
    public interface IMatchInput
    {
        public string Name { get; }
        public string BasePath { get; }
        public string SubName { get; }
        public string Path { get; }
        public string SubPath { get; }
        public Bitmap BaseImage { get; }
        public Bitmap SubImage { get; }
    }
}