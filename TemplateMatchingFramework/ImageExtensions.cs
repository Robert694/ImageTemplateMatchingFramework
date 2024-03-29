﻿using System.Drawing;

namespace TemplateMatchingFramework
{
    public static class ImageExtensions
    {
        public static System.Drawing.Image Overlay(this System.Drawing.Image input, System.Drawing.Image overlay, Point point)
        {
            var clone = new Bitmap(input.Width, input.Height);
            using Graphics g = Graphics.FromImage(clone);
            g.DrawImage(input, new Rectangle(new Point(0, 0), new Size(input.Width, input.Height)));
            g.DrawImage(overlay, new Rectangle(point, new Size(overlay.Width, overlay.Height)));
            return clone;
        }

        public static void DrawRectangle(this System.Drawing.Image input, Rectangle rect)
        {
            using Graphics g = Graphics.FromImage(input);
            g.DrawRectangle(new Pen(Color.Red, 3), rect);
        }

        public static System.Drawing.Image Crop(this System.Drawing.Image image, Rectangle cropArea)
        {
            var bmpImage = (Bitmap)image;
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        public static System.Drawing.Image Crop(this Bitmap image, Rectangle cropArea)
        {
            return image.Clone(cropArea,image.PixelFormat);
        }
    }
}