using System;
using System.Collections;
using System.Drawing;
using System.Numerics;
using Microsoft.Toolkit.HighPerformance;

namespace TemplateMatchingFramework
{
    public class ImageInfo3DMemory : IComparer
    {
        public Memory2D<Vector4> Values { get; }

        public ImageInfo3DMemory(Memory2D<Vector4> values)
        {
            Values = values;
        }

        public Memory2D<Vector4> SubVector(int x, int y, int width, int height) => Values[x..(x + width), y..(y + height)];

        public ImageInfo3DMemory SubImageInfo(int x, int y, int width, int height) => new(SubVector(x, y, width, height));

        public static Memory2D<Vector4> ImageToVector(Bitmap image, bool dispose = true)
        {
            var result = new Vector4[image.Width, image.Height];
            for (int x = 0; x < image.Width; x++)
            for (int y = 0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, y);
                result[x, y] = new Vector4(p.A, p.R, p.G, p.B);
            }

            if (dispose) image.Dispose();
            return result;
        }

        public static float ComputeDifference(ImageInfo3DMemory a, ImageInfo3DMemory b)
        {
            if (a.Values.Width != b.Values.Width || a.Values.Height != b.Values.Height)
                throw new Exception($"Images contain different amount of pixels ({a.Values.Width}x{a.Values.Height}) != ({b.Values.Width}x{b.Values.Height})");
            var aSpan = a.Values.Span;
            var bSpan = b.Values.Span;
            float sum = 0;
            for (int x = 0; x < a.Values.Width; x++)
            for (int y = 0; y < a.Values.Height; y++)
            {
                sum += aSpan[y, x].ManhattanDistance(bSpan[y, x]); //x and y flipped
            }
            return sum;
        }

        public static float ComputeDifferenceEuclidean(ImageInfo3DMemory a, ImageInfo3DMemory b)
        {
            if (a.Values.Width != b.Values.Width || a.Values.Height != b.Values.Height) 
                throw new Exception($"Images contain different amount of pixels ({a.Values.Width}x{a.Values.Height}) != ({b.Values.Width}x{b.Values.Height})");

            var aSpan = a.Values.Span;
            var bSpan = b.Values.Span;
            float sum = 0;
            for (int x = 0; x < a.Values.Width; x++)
            for (int y = 0; y < a.Values.Height; y++)
            {
                sum += Vector4.Distance(aSpan[y, x], bSpan[y, x]); //x and y flipped
            }
            return sum;
        }

        public static float ComputeDifferenceEuclideanSquared(ImageInfo3DMemory a, ImageInfo3DMemory b)
        {
            if (a.Values.Width != b.Values.Width || a.Values.Height != b.Values.Height)
                throw new Exception($"Images contain different amount of pixels ({a.Values.Width}x{a.Values.Height}) != ({b.Values.Width}x{b.Values.Height})");

            var aSpan = a.Values.Span;
            var bSpan = b.Values.Span;
            float sum = 0;
            for (int x = 0; x < a.Values.Width; x++)
            for (int y = 0; y < a.Values.Height; y++)
            {
                sum += Vector4.DistanceSquared(aSpan[y, x], bSpan[y, x]); //x and y flipped
            }
            return sum;
        }

        public Image ToImage()
        {
            var span = Values.Span;
            Bitmap bmp = new Bitmap(span.Width, span.Height);
            for (int x = 0; x < span.Width; x++)
            for (int y = 0; y < span.Height; y++)
            {
                var v = span[y, x];//x and y flipped
                bmp.SetPixel(x, y, Color.FromArgb((int)v.X, (int) v.Y, (int) v.Z, (int) v.W));
            }
            return bmp;
        }

        public int Compare(object x, object y)
        {
            var xx = (ImageInfo3DMemory)x;
            var yy = (ImageInfo3DMemory)y;
            return (int)ComputeDifference(xx, yy);
        }
    }
}