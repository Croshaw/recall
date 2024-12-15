using System.ComponentModel;

namespace Call.GUI.Controls;

public class PagesPanel : Panel
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control? Page { get; private set; }

    private Dictionary<string, Control> Pages { get; } = new();

    public void AddPage(Control page)
    {
        ArgumentNullException.ThrowIfNull(page);
        AddPage(page.Name, page);
    }

    public void AddPage(string name, Control page)
    {
        ArgumentNullException.ThrowIfNull(page);
        Pages.Add(name, page);
        if (Page is null)
            SwitchPage(name);
    }

    public void SwitchPage(string name)
    {
        if (!Pages.TryGetValue(name, out var page)) return;
        if (Page == page)
            return;
        Controls.Remove(Page);
        Controls.Add(page);
        Page = page;
    }
}