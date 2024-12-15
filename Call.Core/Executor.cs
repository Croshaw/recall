using Call.Core.Configuration;
using Call.Core.Interpreter;
using Call.Core.Lexing;
using Call.Core.Syntax;
using Console = Call.Core.Configuration.Console;

namespace Call.Core;

public class Executor
{
    public IReadOnlyDictionary<TableType, List<string>> Tables { get; }
    public IReadOnlyList<Token> Tokens { get; }
    private Lexer _lexer;
    private Syntaxer _syntaxer;
    private Console _console;

    public Executor(Console console, string source) : this(console, Settings.Default, source)
    {
    }

    public Executor(Console console, Settings settings, string source)
    {
        _console = console;
        _lexer = new Lexer(source, settings, console);
        Tables = settings.Tables;
        Tokens = _lexer.Tokens;
        _syntaxer = new Syntaxer(settings, Tokens, console);
    }

    public Task Execute()
    {
        return Task.Run(() =>
        {
            if (_lexer.HasErrors || _syntaxer.HasErrors)
                return;
            Interpretator.Execute(_syntaxer.Actions, _syntaxer.Variables, _syntaxer.Values, _console);
        });
    }

    // public static Task<Executor> Execute(Console console, string source, Settings? settings = null)
    // {
    //     var lexer = new Lexer(source, settings, console);
    //
    //     return Task.Run(() => new Executor(console, settings ?? Settings.Default, source));
    // }
}