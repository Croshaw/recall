namespace Call.GUI.Style;

public class ColorScheme
{
    public static ColorScheme Dark = new(ColorTranslator.FromHtml("#20222c"), ColorTranslator.FromHtml("#1c1e26"),
        ColorTranslator.FromHtml("#a8ac9f"), ColorTranslator.FromHtml("#a8ac9f"), ColorTranslator.FromHtml("#a8ac9f"),
        ColorTranslator.FromHtml("#323749"), ColorTranslator.FromHtml("#ffffff"), ColorTranslator.FromHtml("#a8ac9f"),
        ColorTranslator.FromHtml("#232530"));

    // public static Theme Light = new(ColorTranslator.FromHtml("#"), ColorTranslator.FromHtml("#"),
    //     ColorTranslator.FromHtml("#"), ColorTranslator.FromHtml("#"), ColorTranslator.FromHtml("#"),
    //     ColorTranslator.FromHtml("#"), ColorTranslator.FromHtml("#"), ColorTranslator.FromHtml("#"),
    //     ColorTranslator.FromHtml("#"));

    public Color Background { get; init; }
    public Color AltBackground { get; init; }
    public Color Text { get; init; }

    public Color AltText { get; init; }

    public Color DisabledText { get; init; }
    public Color Accent { get; init; }
    public Color AltAccent { get; init; }
    public Color Border { get; init; }
    public Color Divider { get; init; }

    public ColorScheme(Color background, Color altBackground, Color text, Color altText,
        Color disabledText, Color accent, Color altAccent, Color border, Color divider)
    {
        Background = background;
        AltBackground = altBackground;
        Text = text;
        AltText = altText;
        DisabledText = disabledText;
        Accent = accent;
        AltAccent = altAccent;
        Border = border;
        Divider = divider;
    }
}