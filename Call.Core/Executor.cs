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

    public Executor(Console console, string source) : this(console, Settings.Default, source)
    {
    }

    public Executor(Console console, Settings settings, string source)
    {
        var lexer = new Lexer(source, settings, console);
        Tables = settings.Tables;
        Tokens = lexer.Tokens;
        var syntaxer = new Syntaxer(settings, Tokens, console);
        if (lexer.HasErrors || syntaxer.HasErrors)
            return;
        Interpretator.Execute(syntaxer.Actions, syntaxer.Variables, syntaxer.Values, console);
    }
}