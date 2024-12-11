using System.ComponentModel;

namespace Call.GUI.Controls;

public class PositionForRTB : Label
{
    private RichTextBox? _target;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RichTextBox? Target
    {
        get => _target;
        set
        {
            InitTarget(value);
            _target = value;
        }
    }

    public new string Text => base.Text;
    private bool _forwardSelect;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Format { get; set; } = "$row$:$col$";

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string SelectionFormat { get; set; } = "$row$:$col$($sel$ chars)";

    private void InitTarget(RichTextBox? newTarget)
    {
        if (_target is not null)
        {
            _target.TextChanged -= Changed;
            _target.SelectionChanged -= Changed;
            _target.MouseClick -= Changed;
        }

        if (newTarget is null) return;
        newTarget.TextChanged += Changed;
        newTarget.SelectionChanged += Changed;
        newTarget.MouseClick += Changed;

        newTarget.MouseUp += (sender, e) =>
        {
            var end = newTarget.GetCharIndexFromPosition(e.Location);
            _forwardSelect = newTarget.SelectionStart < end;
            Upd();
        };
        newTarget.KeyDown += (sender, e) =>
        {
            if (e.Shift)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left)
                    _forwardSelect = false;
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)
                    _forwardSelect = true;
            }
        };
    }

    private void Changed(object? sender, EventArgs e)
    {
        if (_target is not null)
            Upd();
    }

    private void Upd()
    {
        var curInd = _target!.SelectionStart;
        if (_forwardSelect)
            curInd += _target!.SelectionLength;
        var offset = 0;
        var line = 1;
        var lines = _target.Lines;
        for (var i = 0; i < lines.Length; i++)
        {
            if (curInd >= offset && curInd <= offset + lines[i].Length)
            {
                line = i + 1;
                break;
            }

            offset += lines[i].Length + 1;
        }

        var col = 1 + curInd - offset;
        base.Text = (_target.SelectionLength > 0 ? SelectionFormat : Format).Replace("$row$", line.ToString())
            .Replace("$col$", col.ToString())
            .Replace("$pos$", curInd.ToString()).Replace("$sel$", _target.SelectionLength.ToString());
    }
}