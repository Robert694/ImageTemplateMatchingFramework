using System;
using System.Drawing;
using System.Threading.Tasks;
using TemplateMatchingFramework.ImageInfo;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.Match
{
    public class ParallelTemplateMatching : IMatchProcessor
    {
        public enum OptimizationLevel
        {
            None,
            SkipOne,
            SkipTwo,
            Grid,
            GridDots,
            Checker,
        }

        public enum DifferenceType
        {
            Manhattan,
            Euclidean,
            EuclideanSquared
        }

        /// <summary>
        /// Offset heatmap to center of sub image
        /// </summary>
        public bool CenterOffset = true;
        /// <summary>
        /// Enables console output / title output
        /// </summary>
        public bool ConsoleOutput = false;

        /// <summary>
        /// Only enable if you're okay with reduced accuracy (Skips pixels)
        /// </summary>
        public OptimizationLevel OptimizeLevel = OptimizationLevel.None;

        /// <summary>
        /// Only disable for debugging (Will reduce accuracy drastically if using any optimization)
        /// </summary>
        public bool OptimizeSmoothing = true;

        /// <summary>
        /// Max degrees of parallelism (-1 = auto)
        /// </summary>
        public int MaxDegreeOfParallelism = 1;

        public DifferenceType Difference = DifferenceType.Manhattan;

        public IMatchResult Process(IMatchInput input)
        {
            Func<ImageInfo3DMemory, ImageInfo3DMemory, float> difference = Difference switch
            {
                DifferenceType.Manhattan => ImageInfo3DMemory.ComputeDifference,
                DifferenceType.Euclidean => ImageInfo3DMemory.ComputeDifferenceEuclidean,
                DifferenceType.EuclideanSquared => ImageInfo3DMemory.ComputeDifferenceEuclideanSquared,
                _ => ImageInfo3DMemory.ComputeDifference,
            };
            bool optimize = OptimizeLevel != OptimizationLevel.None;

            var inputHeight = input.BaseImage.Height;
            var inputWidth = input.BaseImage.Width;
            var totalInputPixels = inputWidth * inputHeight;
            var subHeight = input.SubImage.Height;
            var subWidth = input.SubImage.Width;
            Point bestPoint = Point.Empty;
            var bestValue = float.MaxValue;
            int count = 0;
            int widthOffset = CenterOffset ? input.SubImage.Width / 2 : 0;
            int heightOffset = CenterOffset ? input.SubImage.Height / 2 : 0;
            var bp = new BestPointCalculator(inputWidth, inputHeight);
            var imageInfo = new ImageInfo3DMemory(ImageInfo3DMemory.ImageToVector(input.BaseImage, false));
            var subImageInfo = new ImageInfo3DMemory(ImageInfo3DMemory.ImageToVector(input.SubImage, false));

            var widthBounds = inputWidth - subWidth;
            var heightBounds = inputHeight - subHeight;
            Parallel.For(0, totalInputPixels, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, i =>
            {
                if (ConsoleOutput && count++ % (totalInputPixels / 100) == 0) Console.Title = $"{(float)count / totalInputPixels:P}";
                //convert 1D to 2D
                int w = i % inputWidth;
                int h = i / inputWidth;
 
                if (w > widthBounds || h > heightBounds) return;//Don't leave bounds
                switch (OptimizeLevel) //Optimization
                {
                    case OptimizationLevel.SkipOne when i % 2 != 0:
                        return;
                    case OptimizationLevel.SkipTwo when i % 3 != 0:
                        return;
                    case OptimizationLevel.Grid when w % 2 != 0 && h % 2 != 0:
                        return;
                    case OptimizationLevel.GridDots when w % 2 != 0 || h % 2 != 0:
                        return;
                    case OptimizationLevel.Checker when h % 2 == 0 && i % 2 == 0 || h % 2 != 0 && i % 2 != 0:
                        return;
                    case OptimizationLevel.None:
                        break;
                }
                var crop = imageInfo.SubImageInfo(w, h, subWidth, subHeight);
                var diff = difference(crop, subImageInfo);
                bp[w + widthOffset, h + heightOffset] = diff;
            });
            if (ConsoleOutput) Console.Title = $"{(float)1:P}";

            if(optimize && OptimizeSmoothing)bp.Calculate();
            bestPoint = optimize
                ? bp.GetMinPointAverage(out bestValue).GetValueOrDefault(Point.Empty)
                : bp.GetMinPoint(out bestValue).GetValueOrDefault(Point.Empty);
            bestPoint.X -= widthOffset;
            bestPoint.Y -= heightOffset;
            return new CalcMatchResult(bestPoint, bestValue, bp);
        }
    }
}