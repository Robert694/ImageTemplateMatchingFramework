using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using AForge.Imaging;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;
using Image = System.Drawing.Image;

namespace TemplateMatchingFramework.Match
{
    public class AForgeExhaustiveTemplateMatching : IMatchProcessor
    {
        public float SimilarityThreshold = 0.921f;

        /// <summary>
        /// Max degrees of parallelism (-1 = auto)
        /// </summary>
        public int MaxDegreeOfParallelism = -1;

        public IMatchResult Process(IMatchInput input)
        {
            var tm = new ModifiedAForgeExhaustiveTemplateMatching(SimilarityThreshold){MaxDegreeOfParallelism = MaxDegreeOfParallelism};
            // find all matchings with specified above similarity

            using var image = ConvertToFormat(input.BaseImage, PixelFormat.Format24bppRgb);
            using var template = ConvertToFormat(input.SubImage, PixelFormat.Format24bppRgb);
            TemplateMatch[] matchings = tm.ProcessImage(image, template);
            var result = matchings.FirstOrDefault();
            return result == null
                ? new TemplateMatchResult(Point.Empty, 0, tm)
                : new TemplateMatchResult(result.Rectangle.Location, result.Similarity, tm);
        }

        private static Bitmap ConvertToFormat(System.Drawing.Image image, PixelFormat format)
        {
            var copy = new Bitmap(image.Width, image.Height, format);
            using var gr = Graphics.FromImage(copy);
            gr.DrawImage(image, new Rectangle(0, 0, copy.Width, copy.Height));
            return copy;
        }
    }
}