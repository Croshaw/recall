using Call.Core.Configuration;
using Call.GUI.Controls;
using Call.GUI.Style;

namespace Call.GUI.Common;

public static class CreateUtils
{
    public static ButtonGroup CreateButtonGroup(DockStyle dock = DockStyle.None,
        FlowDirection flowDirection = FlowDirection.LeftToRight, int gap = 15, Padding? padding = null)
    {
        if (padding is null)
            padding = new Padding(5, 0, 5, 0);
        return new ButtonGroup()
        {
            Dock = dock,
            AutoSize = true,
            FlowDirection = flowDirection,
            Padding = padding.Value,
            Gap = gap
        };
    }

    public static Button CreateButton(string? text)
    {
        var button = new Button()
        {
            Text = text ?? "",
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 }
        };
        return button;
    }

    public static Button CreateIconicButton(string? text, string iconName)
    {
        var button = CreateButton(text);
        button.BackgroundImage = Image.FromFile("Icons/" + iconName);
        button.BackgroundImageLayout = ImageLayout.Zoom;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    public static DataGridView CreateTable(int columns)
    {
        var dgv = new DataGridView()
        {
            Dock = DockStyle.Fill
        };

        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.ReadOnly = true;
        dgv.RowHeadersVisible = dgv.AllowUserToOrderColumns = dgv.ColumnHeadersVisible =
            dgv.AllowUserToAddRows = dgv.AllowUserToResizeRows = dgv.AllowUserToDeleteRows = false;
        for (var i = 0; i < columns; i++)
            dgv.Columns.Add(i.ToString(), i.ToString());
        return dgv;
    }

    public static Panel WrapControlWithName(Control control, string name)
    {
        var label = new Label()
        {
            Text = name,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleLeft
            // AutoSize = false,
        };
        var panel = new Panel()
        {
            Dock = DockStyle.Fill
        };
        panel.Controls.Add(control);
        panel.Controls.Add(label);
        return panel;
    }
}

public static class ModifyUtils
{
    public static void LinkSeparatorToControl(Control separator, Control target)
    {
        var horizontal = separator.Dock is DockStyle.Left or DockStyle.Right;
        var inversion = separator.Dock is DockStyle.Right or DockStyle.Bottom ? 1 : -1;
        separator.Cursor = horizontal ? Cursors.SizeWE : Cursors.SizeNS;

        var isMouseDown = false;
        var initialPoint = 0;
        var initialSize = 0;

        separator.MouseDown += (sender, e) =>
        {
            isMouseDown = true;
            initialPoint = horizontal ? Cursor.Position.X : Cursor.Position.Y;
            initialSize = horizontal ? target.Width : target.Height;
        };
        separator.MouseUp += (sender, e) => { isMouseDown = false; };

        separator.MouseMove += (sender, e) =>
        {
            if (isMouseDown)
            {
                var delta = (horizontal ? Cursor.Position.X : Cursor.Position.Y) - initialPoint;
                if (horizontal)
                    target.Width = Math.Max(5, initialSize + delta * inversion);
                else
                    target.Height = Math.Max(5, initialSize + delta * inversion);
            }
        };
    }

    public static void SetupColors(Color? foreColor = null, Color? backColor = null, bool recursive = true,
        params Control[] controls)
    {
        foreach (var control in controls)
        {
            control.ForeColor = foreColor ?? control.ForeColor;
            control.BackColor = backColor ?? control.BackColor;
            if (recursive && control.Controls.Count > 0)
                SetupColors(control.Controls, foreColor, backColor, recursive);
        }
    }

    public static void SetupColors(Control.ControlCollection controls, Color? foreColor = null, Color? backColor = null,
        bool recursive = true)
    {
        foreach (var control in controls)
            if (control is Control ctrl)
            {
                ctrl.ForeColor = foreColor ?? ctrl.ForeColor;
                ctrl.BackColor = backColor ?? ctrl.BackColor;
                if (recursive && ctrl.Controls.Count > 0)
                    SetupColors(ctrl.Controls, foreColor, backColor, recursive);
            }
    }
}