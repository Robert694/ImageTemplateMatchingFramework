using System;
using System.Drawing;
using System.IO;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;

namespace TemplateMatchingFramework.ResultProcessor
{
    public class DefaultFlippedResultProcessor : IMatchResultProcessor
    {
        public RotateFlipType Type { get; }

        public DefaultFlippedResultProcessor(RotateFlipType type)
        {
            Type = type;
        }
        public bool CanProcess(IMatchInput input, IMatchResult result) => true;

        public void Process(IMatchInput input, IMatchResult result)
        {
            var resultDir = Path.Combine(input.BasePath, "Results");
            Directory.CreateDirectory(resultDir);
            var filename = Path.Combine(resultDir, $"{input.Name}_{input.SubName}_{Enum.GetName(Type)}.png");
            using Image image = input.BaseImage.Overlay(input.SubImage, result.Point);
            image.RotateFlip(Type);
            image.Save(filename);
        }
    }
}