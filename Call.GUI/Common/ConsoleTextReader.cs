namespace Call.GUI.Common;

public class ConsoleTextReader : TextReader
{
    public delegate void ReadEventHandler();

    public event ReadEventHandler? OnReadStart;
    public event ReadEventHandler? OnReadEnd;
    private readonly Lock _lock = new();
    private string? _line;
    private int _index;
    private readonly ManualResetEvent _lineAvailable = new(false);

    public override int Read()
    {
        lock (_lock)
        {
            OnReadStart?.Invoke();
            _lineAvailable.WaitOne();
            if (_line == null)
            {
                _lineAvailable.Reset();
                return -1;
            }

            var result = _line[_index];
            _index++;
            OnReadEnd?.Invoke();
            if (_index <= _line.Length) return result;
            _line = null;
            _lineAvailable.Reset();
            return result;
        }
    }

    public override string? ReadLine()
    {
        while (true)
        {
            OnReadStart?.Invoke();
            _lineAvailable.WaitOne();

            lock (_lock)
            {
                if (_line == null) continue;
                var result = _line[_index..];
                _line = null;
                _lineAvailable.Reset();
                OnReadEnd?.Invoke();
                return result;
            }
        }
    }

    public void Put(string? line)
    {
        lock (_lock)
        {
            _index = 0;
            _line = line;
            _lineAvailable.Set();
        }
    }
}