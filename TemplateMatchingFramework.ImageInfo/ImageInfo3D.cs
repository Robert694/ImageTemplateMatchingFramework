using System.Collections;
using System.Numerics;

namespace TemplateMatchingFramework.ImageInfo;

public class ImageInfo3D(Vector4[,] values) : IComparer
{
    public int Width { get; } = values.GetLength(0);
    public int Height { get; } = values.GetLength(1);
    public Vector4[,] Values { get; } = values;

    public Vector4[,] SubVector(int x, int y, int width, int height)
    {
        var result = new Vector4[width, height];
        for (int w = 0; w < width; w++)
        for (int h = 0; h < height; h++)
        {
            result[w, h] = Values[w + x, h + y];
        }

        return result;
    }

    public ImageInfo3D SubImageInfo(int x, int y, int width, int height) => new(SubVector(x, y, width, height));

    public static Vector4[,] ImageToVector(System.Drawing.Bitmap image, bool dispose = true)
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

    public static float ComputeDifference(ImageInfo3D a, ImageInfo3D b)
    {
        if (a.Width != b.Width || a.Height != b.Height) throw new Exception("Images contain different amount of pixels");

        float sum = 0;
        for (int x = 0; x < a.Width; x++)
            for (int y = 0; y < a.Height; y++)
            {
                sum += a.Values[x, y].ManhattanDistance(b.Values[x, y]);
            }
        return sum;
    }

    public static float ComputeDifferenceEuclidean(ImageInfo3D a, ImageInfo3D b)
    {
        if (a.Width != b.Width || a.Height != b.Height) throw new Exception("Images contain different amount of pixels");

        float sum = 0;
        for (int x = 0; x < a.Width; x++)
            for (int y = 0; y < a.Height; y++)
            {
                sum += Vector4.Distance(a.Values[x, y], b.Values[x, y]);
            }
        return sum;
    }

    public static float ComputeDifferenceEuclideanSquared(ImageInfo3D a, ImageInfo3D b)
    {
        if (a.Width != b.Width || a.Height != b.Height) throw new Exception("Images contain different amount of pixels");

        float sum = 0;
        for (int x = 0; x < a.Width; x++)
            for (int y = 0; y < a.Height; y++)
            {
                sum += Vector4.DistanceSquared(a.Values[x, y], b.Values[x, y]);
            }
        return sum;
    }

    public int Compare(object x, object y)
    {
        var xx = (ImageInfo3D)x;
        var yy = (ImageInfo3D)y;
        return (int)ComputeDifference(xx, yy);
    }
}