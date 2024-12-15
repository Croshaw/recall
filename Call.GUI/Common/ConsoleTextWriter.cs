using System.Text;

namespace Call.GUI.Common;

public class ConsoleTextWriter : TextWriter
{
    public enum ActionType
    {
        Write,
        WriteLine
    }

    public class WriteEventArgs
    {
        public ActionType ActionType { get; }
        public string? Value { get; }

        public WriteEventArgs(ActionType actionType, string? value)
        {
            ActionType = actionType;
            Value = value;
        }
    }

    public delegate void WriteEventHandler(object sender, WriteEventArgs e);

    public event WriteEventHandler? OnWrite;

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void Write(char value)
    {
        OnWrite?.Invoke(this, new WriteEventArgs(ActionType.Write, value.ToString()));
    }

    public override void Write(string? value)
    {
        OnWrite?.Invoke(this, new WriteEventArgs(ActionType.Write, value));
    }

    public override void WriteLine(char value)
    {
        OnWrite?.Invoke(this, new WriteEventArgs(ActionType.WriteLine, value.ToString()));
    }

    public override void WriteLine(string? value)
    {
        OnWrite?.Invoke(this, new WriteEventArgs(ActionType.WriteLine, value));
    }
}