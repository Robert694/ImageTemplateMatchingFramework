using System.Drawing;
using System.IO;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.ResultProcessor
{
    public class DefaultResultProcessor : IMatchResultProcessor
    {
        /// <summary>
        /// Only set to true when you want a box drawn around the selected area
        /// </summary>
        public bool ShowArea = false;

        public bool CanProcess(IMatchInput input, IMatchResult result) => true;

        public void Process(IMatchInput input, IMatchResult result)
        {
            var resultDir = Path.Combine(input.BasePath, "Results");
            Directory.CreateDirectory(resultDir);
            var filename = Path.Combine(resultDir, $"{input.Name}_{input.SubName}.png");
            using Image image = input.BaseImage.Overlay(input.SubImage, result.Point);
            if (ShowArea)
            {
                image.DrawRectangle(new Rectangle(result.Point, new Size(input.SubImage.Width, input.SubImage.Height)));
            }
            image.Save(filename);
        }
    }
}