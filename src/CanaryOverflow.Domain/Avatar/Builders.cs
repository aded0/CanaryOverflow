using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace CanaryOverflow.Domain.Avatar;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public class AvatarBuilder
{
    private readonly Avatar _avatar;

    public AvatarBuilder(Avatar avatar)
    {
        _avatar = avatar;
    }

    public AvatarBuilder SetSize(int size)
    {
        _avatar.Size = size;
        return this;
    }

    public AvatarBrushBuilder Brush => new(this, _avatar.Brush);
    public AvatarFontBuilder Font => new(this, _avatar.Font);

    public Bitmap Build(Guid id, string text)
    {
        var backgroundColor = GetColor(id);
        var textBrush = GetContrastBrush(backgroundColor);
        
        //todo: add null validation
        var bitmap = new Bitmap(_avatar.Size.Value, _avatar.Size.Value);
        var font = new Font(_avatar.Font.FamilyName, _avatar.Font.EmSize.Value, GraphicsUnit.Pixel);
        var rectangle = new RectangleF(0, 0, _avatar.Size.Value, _avatar.Size.Value);
        var stringFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        graphics.Clear(backgroundColor);
        graphics.DrawString(text, font, textBrush, rectangle, stringFormat);

        return bitmap;
    }
    
    private static Color GetColor(object obj)
    {
        return Color.FromArgb(obj.GetHashCode() | (0xff << 24));
    }
    
    private Brush GetContrastBrush(Color color)
    {
        var lR = StandartToLinear(color.R / 255d);
        var lG = StandartToLinear(color.G / 255d);
        var lB = StandartToLinear(color.B / 255d);

        var lightness = GetPerceivedLightness(0.2126 * lR + 0.7152 * lG + 0.0722 * lB);

        return lightness > 50 ? _avatar.Brush.TextDark : _avatar.Brush.TextLight;
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
}

public class AvatarBrushBuilder
{
    private readonly AvatarBuilder _avatarBuilder;
    private readonly AvatarBrush _avatarBrush;

    public AvatarBrushBuilder(AvatarBuilder avatarBuilder, AvatarBrush avatarBrush)
    {
        _avatarBrush = avatarBrush;
        _avatarBuilder = avatarBuilder;
    }

    public AvatarBuilder SetTextDark(Brush brush)
    {
        _avatarBrush.TextDark = brush;
        return _avatarBuilder;
    }

    public AvatarBuilder SetTextLight(Brush brush)
    {
        _avatarBrush.TextLight = brush;
        return _avatarBuilder;
    }
}

public class AvatarFontBuilder
{
    private readonly AvatarBuilder _avatarBuilder;
    private readonly AvatarFont _avatarFont;

    public AvatarFontBuilder(AvatarBuilder avatarBuilder, AvatarFont avatarFont)
    {
        _avatarFont = avatarFont;
        _avatarBuilder = avatarBuilder;
    }

    public AvatarBuilder SetEmSize(float emSize)
    {
        _avatarFont.EmSize = emSize;
        return _avatarBuilder;
    }

    public AvatarBuilder SetFamilyName(string familyName)
    {
        _avatarFont.FamilyName = familyName;
        return _avatarBuilder;
    }
}
