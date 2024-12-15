namespace Call.Core.Configuration;

public class Console : IDisposable
{
    private bool _isDisposed;
    public TextWriter COUT { get; }
    public TextWriter CER { get; }
    public TextReader CIN { get; }

    public Console(TextWriter cout, TextWriter cer, TextReader cin)
    {
        COUT = cout;
        CER = cer;
        CIN = cin;
        _isDisposed = false;
    }

    public void Clear()
    {
        COUT.WriteLine("clear");
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        COUT.Dispose();
        CER.Dispose();
        CIN.Dispose();
        _isDisposed = true;
    }
}