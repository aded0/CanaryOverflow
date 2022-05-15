using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace CanaryOverflow.Domain.Avatar;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public static class AvatarHelper
{
    private static readonly Brush DarkSolidBrush = new SolidBrush(Color.FromArgb(30, 30, 30));
    private static readonly Brush LightSolidBrush = new SolidBrush(Color.FromArgb(230, 230, 230));

    public static Bitmap Create(Guid guid, string firstName, string lastName)
    {
        var backgroundColor = GetColor(guid);
        var textColor = GetContrastBrush(backgroundColor);
        var avatarString = $"{firstName[0]}{lastName[0]}".ToUpper();
        return Create(avatarString, backgroundColor, textColor);
    }

    private static Color GetColor(object obj)
    {
        return Color.FromArgb(obj.GetHashCode() | (0xff << 24));
    }

    private static Brush GetContrastBrush(Color color)
    {
        var lR = StandartToLinear(color.R / 255d);
        var lG = StandartToLinear(color.G / 255d);
        var lB = StandartToLinear(color.B / 255d);

        var lightness = GetPerceivedLightness(0.2126 * lR + 0.7152 * lG + 0.0722 * lB);

        return lightness > 50 ? DarkSolidBrush : LightSolidBrush;
    }

    private static double StandartToLinear(double colorChannel)
    {
        return colorChannel <= 0.04045
            ? colorChannel / 12.92
            : Math.Pow((colorChannel + 0.055) / 1.055, 2.4);
    }

    private static double GetPerceivedLightness(double luminance)
    {
        return luminance <= 0.008856
            ? luminance * 903.3
            : Math.Pow(luminance, 0.33) * 116 - 16;
    }

    private static Bitmap Create(string text, Color backgroundColor, Brush textColor)
    {
        var bitmap = new Bitmap(128, 128);

        var font = new Font("Roboto", 64, GraphicsUnit.Pixel);
        var rectangle = new RectangleF(0, 0, 128, 128);
        var stringFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        graphics.Clear(backgroundColor);
        graphics.DrawString(text, font, textColor, rectangle, stringFormat);

        return bitmap;
    }
}
