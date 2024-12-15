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
        return button;
    }

    public static DataGridView CreateTable(int columns, ThemeManager themeManager)
    {
        var dgv = new DataGridView()
        {
            Dock = DockStyle.Fill
        };
        dgv.DefaultCellStyle.BackColor = themeManager.Theme.ColorScheme.Background;
        dgv.DefaultCellStyle.SelectionBackColor = themeManager.Theme.ColorScheme.Background;
        dgv.BorderStyle = BorderStyle.None;
        dgv.BackgroundColor = themeManager.Theme.ColorScheme.AltBackground;
        dgv.GridColor = themeManager.Theme.ColorScheme.Border;
        dgv.ForeColor = themeManager.Theme.ColorScheme.Text;

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
    public static void SetupTablePanel(TableLayoutPanel tablePanel, TableWrapper wrapper, ThemeManager themeManager)
    {
        var reserved = CreateUtils.CreateTable(2, themeManager);
        var sep = CreateUtils.CreateTable(2, themeManager);
        var num = CreateUtils.CreateTable(2, themeManager);
        var id = CreateUtils.CreateTable(2, themeManager);
        reserved.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        reserved.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        sep.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        sep.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        num.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        num.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        id.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        id.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        wrapper.Add(TableType.Reserved, reserved);
        wrapper.Add(TableType.Separators, sep);
        wrapper.Add(TableType.Numbers, num);
        wrapper.Add(TableType.Identifiers, id);
        tablePanel.Dock = DockStyle.Fill;
        tablePanel.ColumnCount = 1;
        tablePanel.RowCount = 4;
        tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
        tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
        tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
        tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
        tablePanel.Controls.Add(CreateUtils.WrapControlWithName(reserved, "Reserved"), 0, 0);
        tablePanel.Controls.Add(CreateUtils.WrapControlWithName(sep, "Separators"), 0, 1);
        tablePanel.Controls.Add(CreateUtils.WrapControlWithName(num, "Numbers"), 0, 2);
        tablePanel.Controls.Add(CreateUtils.WrapControlWithName(id, "Identifiers"), 0, 3);
    }

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