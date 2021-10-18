using System.Drawing;

namespace TemplateMatchingFramework.Input
{
    public class MatchInput : IMatchInput
    {
        public MatchInput(string name, string subName, string basePath, string path, string subPath, Bitmap baseImage, Bitmap subImage)
        {
            Name = name;
            SubName = subName;

            BasePath = basePath;
            Path = path;
            SubPath = subPath;

            BaseImage = baseImage;
            SubImage = subImage;
        }

        public string Name { get; }
        public string BasePath { get; }
        public string SubName { get; }
        public string Path { get; }
        public string SubPath { get; }
        public Bitmap BaseImage { get; }
        public Bitmap SubImage { get; }

        public override string ToString()
        {
            return $"{Name}/{SubName}";
        }
    }
}