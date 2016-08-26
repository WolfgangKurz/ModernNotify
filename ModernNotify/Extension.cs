using System;
using System.Drawing;

namespace ModernNotify
{
    internal static class Extension
    {
        public static Color RgbColor(int rgb)
        {
            return Color.FromArgb((rgb & 0xFF0000) >> 16, (rgb & 0xFF00) >> 8, rgb & 0xFF);
        }
    }
}
