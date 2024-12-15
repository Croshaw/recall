using System.Text;
using Call.GUI.Common;
using Console = Call.Core.Configuration.Console;

namespace Call.GUI.Controls;

public class Terminal : RichTextBox
{
    private int _currentLine;
    private int _lastCaretPos;
    private int CountViewLines => GetLineFromCharIndex(TextLength);
    public Console Console { get; }
    public new bool ReadOnly => base.ReadOnly;
    public override bool Multiline => true;

    public Terminal()
    {
        Reset();
        var cout = new ConsoleTextWriter();
        var cer = new ConsoleTextWriter();
        var cin = new ConsoleTextReader();
        Console = new Console(cout, cer, cin);
        cout.OnWrite += (_, e) => { Invoke(() => AppendText(e, Color.Black)); };
        cer.OnWrite += (_, e) => { Invoke(() => AppendText(e, Color.Red)); };
        cin.OnReadStart += () => base.ReadOnly = false;
        cin.OnReadEnd += () => base.ReadOnly = true;
    }

    public void Reset()
    {
        Clear();
        _currentLine = 0;
        _lastCaretPos = 0;
        SelectionStart = 0;
        base.ReadOnly = true;
    }

    private void AppendText(ConsoleTextWriter.WriteEventArgs args, Color color)
    {
        var start = TextLength;
        Text += args.Value;
        if (args.ActionType == ConsoleTextWriter.ActionType.WriteLine)
            Text += Environment.NewLine;
        var end = TextLength;

        Select(start, end - start);
        SelectionColor = color;
        SelectionLength = 0;
        if (args.Value != null &&
            (args.ActionType == ConsoleTextWriter.ActionType.WriteLine || args.Value.Contains("\n")))
            OnLineAdd();
    }

    private bool IsRestrict()
    {
        var curLine = GetLineFromCharIndex(SelectionStart);
        return CountViewLines >= curLine && curLine > _currentLine;
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
            if (keyData is Keys.Back or Keys.Left)
                if (GetLineFromCharIndex(Math.Max(SelectionStart - 1, 0)) != _currentLine)
                    return true;
        }

        if (keyData == Keys.Enter)
            if (OnLineAdd(true))
                return true;

        return base.ProcessCmdKey(ref m, keyData);
    }

    private bool OnLineAdd(bool manual = false)
    {
        ScrollToCaret();
        var line = Lines[^1];
        if (line.Length == 0)
            return true;
        if (manual) (Console.CIN as ConsoleTextReader)?.Put(line);
        switch (line)
        {
            case "":
                return true;
            case "cls":
            case "clear":
                Reset();
                return true;
            default:
                _currentLine = CountViewLines + 1;
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
        if (IsRestrict()) return;
        var newLine = GetLineFromCharIndex(SelectionStart);
        if (newLine != _currentLine)
            SelectionStart = _lastCaretPos;
    }
}