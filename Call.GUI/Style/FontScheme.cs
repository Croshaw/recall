namespace Call.GUI.Style;

public class FontScheme
{
    public static readonly string DefaultFontFamily = "Consolas";

    public static readonly FontScheme Default = new(
        new Font(DefaultFontFamily, 15f, FontStyle.Bold),
        new Font(DefaultFontFamily, 15f, FontStyle.Regular),
        new Font(DefaultFontFamily, 15f, FontStyle.Bold),
        new Font(DefaultFontFamily, 15f, FontStyle.Regular),
        new Font(DefaultFontFamily, 10f, FontStyle.Regular));

    public Font RunFont { get; }
    public Font CodeFont { get; }
    public Font TitleFont { get; }
    public Font MenuFont { get; }
    public Font TreeFont { get; }

    public FontScheme(Font runFont, Font codeFont, Font titleFont, Font menuFont, Font treeFont)
    {
        RunFont = runFont;
        CodeFont = codeFont;
        TitleFont = titleFont;
        MenuFont = menuFont;
        TreeFont = treeFont;
    }
}