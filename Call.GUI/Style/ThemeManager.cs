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

    public void ApplyTheme(Control control, bool primary)
    {
        var backColor = primary
            ? Theme.ColorScheme.Background
            : Theme.ColorScheme.AltBackground;
        var textColor = primary
            ? Theme.ColorScheme.Text
            : Theme.ColorScheme.AltText;
        control.ForeColor = textColor;
        control.BackColor = backColor;
        switch (control)
        {
            case DataGridView dgv:
                dgv.BorderStyle = BorderStyle.None;
                dgv.DefaultCellStyle.BackColor = backColor;
                dgv.DefaultCellStyle.SelectionBackColor = backColor;
                dgv.DefaultCellStyle.SelectionForeColor = textColor;
                dgv.GridColor = Theme.ColorScheme.Border;
                dgv.BackgroundColor = backColor;
                break;
            case Button button:
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                break;
        }
    }

    public void SetupControl(Control control, bool primary)
    {
        ApplyTheme(control, primary);
        ThemeChanged += (_, _) => ApplyTheme(control, primary);
    }
}