using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace TemplateMatchingFramework
{
    public class BestPointCalculator
    {
        public int Width { get; }
        public int Height { get; }
        private float?[,] Values { get; }
        private float?[,] CalculatedValues { get; }
        private bool Calculated = false;

        public BestPointCalculator(int width, int height)
        {
            Width = width;
            Height = height;
            Values = new float?[Width, Height];
            CalculatedValues = new float?[Width, Height];
        }

        public float? this[int x, int y]
        {
            get => Values[x, y];
            set => Values[x, y] = value;
        }

        public Point? GetMinPoint(out float lowest)
        {
            Point? result = null;
            lowest = float.MaxValue;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var v = Values[x, y];
                if (!v.HasValue || !(v.Value < lowest)) continue;
                lowest = v.Value;
                result = new Point(x, y);
            }
            return result;
        }

        public Point? GetMinPointAverage(out float lowest)
        {
            Point? result = null;
            lowest = float.MaxValue;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var v = CalculatedValues[x, y];
                if (!v.HasValue || !(v.Value < lowest)) continue;
                lowest = v.Value;
                result = new Point(x, y);
            }
            return result;
        }

        public IEnumerable<Vector2> GetCalculatedPositions(float? value)
        {
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (CalculatedValues[x, y] == value)
                    yield return new Vector2(x, y);
            }
        }

        public void GetMinMax(out float min, out float max)
        {
            min = float.MaxValue;
            max = 0;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var v = Values[x, y];
                if (!v.HasValue) continue;

                if (v.Value < min)
                    min = v.Value;

                if (v.Value > max)
                    max = v.Value;
            }
        }

        public void GetMinMaxAverage(out float min, out float max)
        {
            min = float.MaxValue;
            max = 0;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var v = CalculatedValues[x, y];
                if (!v.HasValue) continue;

                if (v.Value < min)
                    min = v.Value;

                if (v.Value > max)
                    max = v.Value;
            }
        }

        public void Calculate()
        {
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                CalculatedValues[x, y] = CalculatePoint(x, y);
            Calculated = true;
        }

        public float? CalculatePoint(int x, int y)
        {
            float sum = 0;
            int count = 0;
            const int size = 1;
            for(int xx = x - size; xx <= x + size; xx++)
            for (int yy = y - size; yy <= y + size; yy++)
            {
                if(!Contains(xx, yy) || !Values[xx, yy].HasValue) continue;
                count++;
                sum += Values[xx, yy].Value;
            }
            return count == 0 ? null : sum / count;
        }

        public void Print()
        {
            for (int x = 0; x < Width; x++)
            {
                Console.WriteLine();
                for (int y = 0; y < Height; y++)
                {
                    var v = (int?)Values[x, y];
                    Console.Write($"{(v == null ? "Null" : v.ToString()),8}");
                }
            }
            Console.WriteLine();
        }

        public void PrintCalculated()
        {
            for (int x = 0; x < Width; x++)
            {
                Console.WriteLine();
                for (int y = 0; y < Height; y++)
                {
                    var v = (int?) CalculatedValues[x, y];
                    Console.Write($"{(v == null ? "Null" : v.ToString()),8}");
                }
            }
            Console.WriteLine();
        }

        public bool Contains(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        private Bitmap GetValueMask()
        {
            GetMinMax(out var min, out var max);
            Bitmap bmp = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var val = Values[x, y].GetValueOrDefault(max);
                var color = RangeToGrayscale(max, min, val);
                bmp.SetPixel(x, y, color);
            }
            return bmp;
        }

        private Bitmap GetAverageValueMask()
        {
            GetMinMaxAverage(out var min, out var max);
            Bitmap bmp = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var val = CalculatedValues[x, y].GetValueOrDefault(max);
                var color = RangeToGrayscale(max, min, val);
                bmp.SetPixel(x, y, color);
            }
            return bmp;
        }

        public Bitmap GetMask()
        {
            return Calculated ? GetAverageValueMask() : GetValueMask();
        }

        public Bitmap GetHeatmap()
        {
            return Calculated ? GetAverageValueHeatmap() : GetValueHeatmap();
        }

        private Bitmap GetValueHeatmap()
        {
            GetMinMax(out var min, out var max);
            Bitmap bmp = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var val = Values[x, y].GetValueOrDefault(max);
                var color = HsvToColor(max, min, val);
                if (val == max) color = Color.Black;
                if (val == min) color = Color.White;
                bmp.SetPixel(x, y, color);
            }
            return bmp;
        }

        private Bitmap GetAverageValueHeatmap()
        {
            GetMinMaxAverage(out var min, out var max);
            Bitmap bmp = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var val = CalculatedValues[x, y].GetValueOrDefault(max);
                //var color = HsvToColor(0, s, p);
                var color = HsvToColor(max, min, val);
                if (val == max) color = Color.Black;
                if (val == min) color = Color.White;
                bmp.SetPixel(x, y, color);
            }
            return bmp;
        }

        private static (int r, int g, int b) HsvToRgb(float minimum, float maximum, float value)
        {
            var ratio = 2 * (value - minimum) / (maximum - minimum);
            var b = (int)Math.Max(0, 255 * (1 - ratio));
            var r = (int)Math.Max(0, 255 * (ratio - 1));
            var g = 255 - b - r;
            return (r, g, b);
        }

        private static Color HsvToColor(float minimum, float maximum, float value)
        {
            var v = HsvToRgb(minimum, maximum, value);
            return Color.FromArgb(v.r, v.g, v.b);
        }

        private static Color RangeToGrayscale(float min, float max, float value)
        {
            int v = (int)(((value - min) / (max - min)) * 255);
            return Color.FromArgb(v, v, v);
        }

        private static float ScaleNumber(float min, float max, float val)
        {
            return (val - min) / (max - min);
        }

        private static Color GetBlendedColor(int percentage)
        {
            Color red = Color.FromArgb(255, 0, 0);
            Color green = Color.FromArgb(0, 255, 0);
            Color yellow = Color.FromArgb(255, 255, 0);
            return percentage < 50 ? Interpolate(red, yellow, percentage / 50.0) : Interpolate(yellow, green, (percentage - 50) / 50.0);
        }
        
        private static Color Interpolate(Color color1, Color color2, double fraction)
        {
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return Color.FromArgb((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
        }

        private static double Interpolate(double d1, double d2, double fraction)
        {
            //return d1 + (d1 - d2) * fraction;
            return d1 + (d2 - d1) * fraction;
        }
    }
}