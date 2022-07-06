using System.Drawing;

namespace CanaryOverflow.Domain.Avatar;

public class Avatar
{
    private Avatar()
    {
        Brush = new AvatarBrush();
        Font = new AvatarFont();
    }

    public AvatarBrush Brush { get; }
    public int? Size { get; set; }
    public AvatarFont Font { get; }

    public static AvatarBuilder Create()
    {
        return new AvatarBuilder(new Avatar());
    }
}

public class AvatarBrush
{
    public Brush? TextDark { get; set; }
    public Brush? TextLight { get; set; }
}


public class AvatarFont
{
    public string? FamilyName { get; set; }
    public float? EmSize { get; set; }
}



