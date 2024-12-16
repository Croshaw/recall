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
    public IReadOnlyList<string> Polis { get; }
    public IReadOnlyDictionary<string, int> Variables { get; }
    public IReadOnlyList<UniValue> Values { get; }
    public Node<string> Root { get; }

    private readonly Lexer _lexer;

    // private Syntaxer _syntaxer;
    private readonly Parser _parser;
    private readonly Console _console;

    public Executor(Console console, string source) : this(console, Settings.Default, source)
    {
    }

    public Executor(Console console, Settings settings, string source)
    {
        _console = console;
        _lexer = new Lexer(source, settings, console);
        Tables = settings.Tables;
        Tokens = _lexer.Tokens;
        _parser = new Parser(settings, Tokens, console);
        Root = _parser.Pr();
        List<string> actions = [];
        foreach (var action in _parser.Actions)
            if (action is AddressAction addressAction)
                actions.Add($"{addressAction.Type} : {addressAction.Address}");
            else if (action is OperatorAction operatorAction)
                actions.Add(operatorAction.Operator);
            else if (action is UnaryOperatorAction unaryOperatorAction)
                actions.Add(unaryOperatorAction.Operator);
            else if (action is SpecialAction specialAction) actions.Add(specialAction.Type.ToString());
        Variables = _parser.Variables;
        Values = _parser.Values;
        Polis = actions;
    }

    public Task Execute()
    {
        return Task.Run(() =>
        {
            if (_lexer.HasErrors || _parser.HasErrors)
                return;
            Interpretator.Execute(_parser.Actions, _parser.Variables, _parser.Values, _console);
        });
    }

    // public static Task<Executor> Execute(Console console, string source, Settings? settings = null)
    // {
    //     var lexer = new Lexer(source, settings, console);
    //
    //     return Task.Run(() => new Executor(console, settings ?? Settings.Default, source));
    // }
}