using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using TemplateMatchingFramework.Input;
using TemplateMatchingFramework.Result;
using TemplateMatchingFramework.ResultProcessor;

namespace TemplateMatchingFramework.Match
{
    public class ImageProcessor : IDisposable
    {
        public string InputPath { get; }
        public string SubImageFolder { get; }
        public IMatchProcessor BestMatchProcessor { get; }
        public IMatchResultProcessor ResultProcessor { get; }

        public ImageProcessor(
            string path,
            string subImageFolder,
            IMatchProcessor bestMatchProcessor,
            IMatchResultProcessor resultProcessor
            )
        {
            InputPath = path;
            SubImageFolder = subImageFolder;
            BestMatchProcessor = bestMatchProcessor;
            ResultProcessor = resultProcessor;
        }

        public List<IMatchInput> Inputs = new List<IMatchInput>();

        public void InputsToGrayscale()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                var m = Inputs[i];
                Inputs[i] = new MatchInput(m.Name, m.SubName, m.BasePath, m.Path, m.SubPath, MakeGrayscale(m.BaseImage),
                    MakeGrayscale(m.SubImage));
            }
        }

        public static Bitmap MakeGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using Graphics g = Graphics.FromImage(newBitmap);
            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            using ImageAttributes attributes = new ImageAttributes();
            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            return newBitmap;
        }

        public void LoadInputs()
        {
            Directory.CreateDirectory(InputPath);
            foreach (var dir in Directory.GetDirectories(InputPath))
            {
                Directory.CreateDirectory(Path.Combine(dir, SubImageFolder));
                foreach (var baseImagePath in Directory.GetFiles(dir))
                {
                    var name = new DirectoryInfo(dir).Name;
                    var baseImage = (Bitmap)Image.FromFile(baseImagePath);
                    foreach (var subImagePath in Directory.GetFiles(Path.Combine(dir, SubImageFolder)))
                    {
                        var subName = Path.GetFileNameWithoutExtension(subImagePath);
                        var subImage = (Bitmap)Image.FromFile(subImagePath);
                        IMatchInput input = new MatchInput(name, subName, dir, baseImagePath, subImagePath, baseImage, subImage);
                        Inputs.Add(input);
                    }
                }
            }
        }

        public void Start()
        {
            foreach (var i in Inputs)
            {
                OnBeforeInput(i);
                Progress<float> p = new Progress<float>();
                var result = BestMatchProcessor.Process(i);
                OnAfterInput(i, result);
                ResultProcessor.Process(i, result);
            }
        }

        /// <summary>
        /// Called once per input before processing
        /// </summary>
        /// <param name="input"></param>
        public virtual void OnBeforeInput(IMatchInput input)
        {

        }

        /// <summary>
        /// Called once per input after processing
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        public virtual void OnAfterInput(IMatchInput input, IMatchResult result)
        {

        }

        public void Dispose()
        {
            foreach (var i in Inputs)
            {
                i.BaseImage.Dispose();
                i.SubImage.Dispose();
            }
        }
    }
}