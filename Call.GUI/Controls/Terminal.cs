using System.Text;

namespace Call.GUI.Controls;

public class Terminal : RichTextBox
{
    private int _currentLine;
    private int _lastCaretPos;
    private int HzLines => GetLineFromCharIndex(TextLength);

    public Terminal()
    {
        Multiline = true;
        ScrollBars = RichTextBoxScrollBars.Vertical;
        WordWrap = true;
        _currentLine = _lastCaretPos = 0;
    }

    private bool IsRestrict()
    {
        var curLine = GetLineFromCharIndex(SelectionStart);
        return HzLines >= curLine && curLine > _currentLine;
    }

    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        if (!IsRestrict())
        {
            if (keyData == Keys.Up)
                return true;
            if (GetLineFromCharIndex(Math.Max(SelectionStart, 0)) != _currentLine ||
                GetLineFromCharIndex(Math.Max(SelectionStart + SelectionLength, 0)) != _currentLine)
                return true;
            if (keyData == Keys.Back || keyData == Keys.Left)
                if (GetLineFromCharIndex(Math.Max(SelectionStart - 1, 0)) != _currentLine)
                    return true;
        }

        if (keyData == Keys.Enter)
            if (OnLineAdd())
                return true;

        return base.ProcessCmdKey(ref m, keyData);
    }

    protected bool OnLineAdd()
    {
        ScrollToCaret();
        if (Lines.Length == 0)
            return true;
        switch (Lines[^1])
        {
            case "":
                return true;
            case "cls":
            case "clear":
                Clear();
                _currentLine = 0;
                _lastCaretPos = 0;
                SelectionStart = 0;
                return true;
            default:
                _currentLine = HzLines + 1;
                return false;
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0201)
        {
            if (GetLineFromCharIndex(SelectionStart) != _currentLine)
                _lastCaretPos = GetFirstCharIndexFromLine(_currentLine);
            else
                _lastCaretPos = SelectionStart;
        }

        base.WndProc(ref m);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (!IsRestrict())
        {
            var newLine = GetLineFromCharIndex(SelectionStart);
            if (newLine != _currentLine)
                SelectionStart = _lastCaretPos;
        }
    }
}