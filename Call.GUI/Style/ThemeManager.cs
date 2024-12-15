namespace Call.GUI.Style;

public class ThemeManager
{
    private Theme _theme;

    public ThemeManager(Theme theme)
    {
        Theme = theme;
    }

    public delegate void ThemeChangedEventHandler(object sender, Theme newTheme);

    public event ThemeChangedEventHandler? ThemeChanged;

    public Theme Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            ThemeChanged?.Invoke(this, _theme);
        }
    }
}