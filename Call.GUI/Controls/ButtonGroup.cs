using System.ComponentModel;

namespace Call.GUI.Controls;

public class ButtonGroup : Panel
{
    private FlowDirection _flowDirection = FlowDirection.LeftToRight;
    private int _gap;

    public delegate void ActiveButtonChangingHandler(Button? newButton);

    public event ActiveButtonChangingHandler? ActiveButtonChanging;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Button? ActiveButton { get; private set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Gap
    {
        get => _gap;
        set
        {
            _gap = value;
            UpdateGap();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public FlowDirection FlowDirection
    {
        get => _flowDirection;
        set
        {
            _flowDirection = value;
            UpdateDock();
        }
    }

    private DockStyle GetDock()
    {
        return FlowDirection switch
        {
            FlowDirection.TopDown => DockStyle.Top,
            FlowDirection.BottomUp => DockStyle.Bottom,
            FlowDirection.LeftToRight => DockStyle.Left,
            FlowDirection.RightToLeft => DockStyle.Right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Add(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);
        var dock = GetDock();
        button.Dock = dock;
        button.Click += OnButtonClick;
        Controls.Add(button);
        Controls.Add(CreateGap(Gap, dock));
    }

    public void AddRange(params Button[] buttons)
    {
        var dock = GetDock();
        foreach (var button in buttons)
        {
            button.Click += OnButtonClick;
            button.Dock = dock;
            Controls.Add(button);
            Controls.Add(CreateGap(Gap, dock));
        }
    }

    private void UpdateGap()
    {
        foreach (Control control in Controls)
            if (control is Panel panel && panel.Name == "Gap")
                panel.Height = panel.Width = Gap;
    }

    private void UpdateDock()
    {
        var dock = GetDock();
        foreach (Control control in Controls) control.Dock = dock;
    }

    private void OnButtonClick(object? sender, EventArgs e)
    {
        if (sender is not Button button)
            return;
        ActiveButtonChanging?.Invoke(button);
        ActiveButton = button;
    }

    private static Panel CreateGap(int size, DockStyle dock)
    {
        return new Panel { Name = "Gap", Size = new Size(size, size), Dock = dock, BackColor = Color.Transparent };
    }
}