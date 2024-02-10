using System.Numerics;

namespace TemplateMatchingFramework.ImageInfo;

public static class VectorExtensions
{
    public static float ManhattanDistance(this Vector3 a, Vector3 b)
    {
        var dx = Math.Abs(b.X - a.X);
        var dy = Math.Abs(b.Y - a.Y);
        var dz = Math.Abs(b.Z - a.Z);
        return dx + dy + dz;
    }

    public static float ManhattanDistance(this Vector4 a, Vector4 b)
    {
        var dx = Math.Abs(b.X - a.X);
        var dy = Math.Abs(b.Y - a.Y);
        var dz = Math.Abs(b.Z - a.Z);
        var dw = Math.Abs(b.W - a.W);
        return dx + dy + dz + dw;
    }
}