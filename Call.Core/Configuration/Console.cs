namespace Call.Core.Configuration;

public class Console : IDisposable
{
    public TextWriter COUT { get; }
    public TextWriter CER { get; }
    public TextReader CIN { get; }

    public Console(TextWriter cout, TextWriter cer, TextReader cin)
    {
        COUT = cout;
        CER = cer;
        CIN = cin;
    }

    public void Dispose()
    {
        COUT.Dispose();
        CER.Dispose();
        CIN.Dispose();
    }
}