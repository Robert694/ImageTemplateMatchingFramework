using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;

namespace TemplateMatchingFramework
{
    public class ImageInfo : IComparer
    {
        //NOTE: creating normalized histograms of the pixels and comparing them is a way to compare images even if they've been scaled or rotated
        //NOTE: That can also be done to our small images. Though less accurate
        public Vector4[] Values { get; }

        public ImageInfo(Vector4[] values)
        {
            Values = values;
        }

        public Vector4[] SubVector(int x, int y, int width, int height)
        {
            var values = new Vector4[width*height];
            int count = 0;
            for (int xx = x; xx < width; xx++)
            for (int yy = y; yy < height; yy++)
            {
                var i = (xx * width) + yy;
                //if (i < 0 || i > Values.Length) continue;
                values[count++] = Values[i];
            }

            return values;
        }

        public ImageInfo SubImageInfo(int x, int y, int width, int height) =>
            new(SubVector(x, y, width, height));

       
        public static Vector4[] ImageToVector(Bitmap image, bool dispose = true)
        {
            var values = new Vector4[image.Width * image.Height];
            int count = 0;
            for (int x = 0; x < image.Width; x++)
            for (int y = 0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, y);
                values[count++] = new Vector4(p.A, p.R, p.G, p.B);
            }

            if (dispose) image.Dispose();
            return values;
        }

        public static ImageInfo FromFile(string file, int w = 4, int h = 4) => FromBytes(File.ReadAllBytes(file), w, h);

        public static ImageInfo FromBytes(byte[] bytes, int w = 4, int h = 4)
        {
            using var ms = new MemoryStream(bytes);
            using var image = (Bitmap)Image.FromStream(ms);
            return new ImageInfo(ImageToVector(ResizeImage(image, w, h)));
        }

        public static ImageInfo FromImage(Image image, int w = 4, int h = 4) => new(ImageToVector(ResizeImage(image, w, h)));

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            return destImage;
        }

        public static float ComputeDifference(ImageInfo a, ImageInfo b)
        {
            if (a.Values.Length != b.Values.Length) throw new Exception("Images contain different amount of pixels");

            float sum = 0;
            for (int i = 0; i < a.Values.Length; i++)
                sum += a.Values[i].ManhattanDistance(b.Values[i]);
            return sum;
        }

        public static float ComputeDifferenceEuclidean(ImageInfo a, ImageInfo b)
        {
            if (a.Values.Length != b.Values.Length) throw new Exception("Images contain different amount of pixels");

            float sum = 0;
            for (int i = 0; i < a.Values.Length; i++)
                sum += Vector4.Distance(a.Values[i], b.Values[i]);
            return sum;
        }

        public static float ComputeDifferenceEuclideanSquared(ImageInfo a, ImageInfo b)
        {
            if (a.Values.Length != b.Values.Length) throw new Exception("Images contain different amount of pixels");

            float sum = 0;
            for (int i = 0; i < a.Values.Length; i++)
                sum += Vector4.DistanceSquared(a.Values[i], b.Values[i]);
            return sum;
        }

        public int Compare(object x, object y)
        {
            var xx = (ImageInfo)x;
            var yy = (ImageInfo)y;
            return (int)ComputeDifference(xx, yy);
        }
    }
}
