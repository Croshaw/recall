namespace Call.GUI.Style;

public class Theme
{
    public static readonly Theme Dark = new(ColorScheme.Dark, FontScheme.Default);
    public ColorScheme ColorScheme { get; }
    public FontScheme FontScheme { get; }

    public Theme(ColorScheme colorScheme, FontScheme fontScheme)
    {
        ColorScheme = colorScheme;
        FontScheme = fontScheme;
    }
}