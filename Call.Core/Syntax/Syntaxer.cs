using System.Diagnostics;
using Call.Core.Configuration;
using Call.Core.Interpreter;
using Call.Core.Lexing;
using Call.Core.Utilities;
using Console = Call.Core.Configuration.Console;

namespace Call.Core.Syntax;

public class Syntaxer
{
    private readonly Settings _settings;
    private readonly IReadOnlyList<Token> _tokens;
    private readonly Console _console;
    private int _index;
    private Token _currentToken;
    private string _tokenValue;
    private readonly HashSet<ValueKind> _kinds;
    private readonly List<object> _actions;
    private readonly Dictionary<string, int> _variables;
    private readonly List<UniValue> _values;
    public bool HasErrors { get; private set; }

    public Syntaxer(Settings settings, IReadOnlyList<Token> tokens, Console console)
    {
        _settings = settings;
        _tokens = tokens;
        _console = console;
        _kinds = [];
        _actions = [];
        _variables = [];
        _values = [];
        Read();
    }

    public IReadOnlyList<object> Actions => _actions;
    public IReadOnlyDictionary<string, int> Variables => _variables;
    public List<UniValue> Values => _values;

    private Node<string> CreateNode()
    {
        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(1);
        return new Node<string>(frame?.GetMethod()?.Name ?? "");
    }

    private void PrintError(ErrorType errorType, string expected)
    {
        if (!HasErrors)
            HasErrors = true;
        _console.CER.WriteLine(Error.GetMessage(errorType, expected, _currentToken.Position));
    }

    private bool Equal(string value)
    {
        return _tokenValue == value;
    }

    private bool Equal(TableType tableType)
    {
        return _currentToken.Table == tableType;
    }

    private void Read()
    {
        _currentToken = _tokens[_index];
        _tokenValue = _settings.GetTable(_currentToken.Table)[_currentToken.Id];
    }

    private void ReadPrev()
    {
        if (_index - 1 >= 0)
            _index--;
        Read();
    }

    private void ReadNext()
    {
        if (_index + 1 < _tokens.Count)
            _index++;
        else
            throw new EndOfStreamException();
        Read();
    }

    private void ReadNextIfEqualElseThrow(string value)
    {
        try
        {
            if (Equal(value))
                ReadNext();
            else
                PrintError(ErrorType.Expected, value);
        }
        catch (EndOfStreamException e)
        {
            // PrintError(ErrorType.Expected, value);
        }
    }

    private (Node<string>, bool) Value()
    {
        var node = CreateNode();
        if (Equal("not"))
        {
            ReadNext();
            node.Add(_tokenValue);
            node.Add(Value().Item1);
            _actions.Add(new UnaryOperatorAction("not"));
            return (node, true);
        }

        if (Equal("true") || Equal("false") || Equal(TableType.Numbers))
        {
            node.Add(_tokenValue);
            try
            {
                _values.Add(UniValue.Parse(_tokenValue));
                _actions.Add(new AddressAction(AddressAction.AddressType.Value, _values.Count - 1));
                return (node, true);
            }
            catch (Exception ex)
            {
                _console.CER.WriteLine(ex.Message);
            }

            return (node, false);
        }

        if (Equal(TableType.Identifiers))
        {
            node.Add(_tokenValue);
            if (!_variables.ContainsKey(_tokenValue))
                PrintError(ErrorType.Undeclared, _tokenValue);
            else
                _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
            return (node, true);
        }

        if (Equal("("))
        {
            node.Add(_tokenValue);
            ReadNext();
            node.Add(Compare().Item1);
            if (!Equal(")"))
                PrintError(ErrorType.Expected, _tokenValue);
            node.Add(_tokenValue);
            return (node, true);
        }

        return (node, false);
    }

    private (Node<string>, bool) Compare()
    {
        var node = CreateNode();
        var temp = Value();
        node.Add(temp.Item1);
        if (temp.Item2)
        {
            ReadNext();
            if (Equal("*") || Equal("/") || Equal("and") || Equal("+") || Equal("-") || Equal("or") || Equal("=") ||
                Equal("<>") || Equal(">") || Equal("<") || Equal(">=") || Equal("<="))
            {
                node.Add(_tokenValue);
                var @operator = _tokenValue;
                ReadNext();
                temp = Compare();
                node.Add(temp.Item1);
                if (temp.Item2)
                    _actions.Add(new OperatorAction(@operator));
            }

            return (node, true);
        }

        return (node, false);
    }

    private Node<string> Assign()
    {
        var node = CreateNode();
        if (!Equal(TableType.Identifiers))
        {
            PrintError(ErrorType.Expected, "identifier");
            return node;
        }

        node.Add(_tokenValue);

        if (!_variables.ContainsKey(_tokenValue))
        {
            PrintError(ErrorType.Undeclared, _tokenValue);
            return node;
        }

        _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
        ReadNext();
        node.Add(_tokenValue);
        ReadNextIfEqualElseThrow("as");
        node.Add(Compare().Item1);
        _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Assign));
        return node;
    }

    private Node<string> Identifiers()
    {
        var node = CreateNode();
        if (!Equal(TableType.Identifiers))
        {
            PrintError(ErrorType.Expected, "identifier");
            return node;
        }

        node.Add(_tokenValue);

        if (!_variables.ContainsKey(_tokenValue))
        {
            PrintError(ErrorType.Undeclared, _tokenValue);
            return node;
        }

        _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
        ReadNext();
        while (Equal(","))
        {
            ReadNext();
            if (!Equal(TableType.Identifiers))
            {
                PrintError(ErrorType.Expected, "identifier");
                return node;
            }

            node.Add(_tokenValue);
            if (!_variables.ContainsKey(_tokenValue))
            {
                PrintError(ErrorType.Undeclared, _tokenValue);
                return node;
            }

            _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
            ReadNext();
        }

        return node;
    }

    public Node<string> Start()
    {
        var node = CreateNode();
        try
        {
            ReadNextIfEqualElseThrow("program");
            node.Add(Vars());
            node.Add(Body());
        }
        catch (Exception ex)
        {
            _console.CER.WriteLine(ex.Message);
        }

        return node;
    }

    private Node<string> Expression()
    {
        var node = CreateNode();
        node.Add(Compare().Item1);
        while (Equal(","))
        {
            ReadNext();
            node.Add(Compare().Item1);
        }

        return node;
    }

    private Node<string> Oper()
    {
        var node = CreateNode();
        if (Equal("["))
        {
            ReadNext();
            node.Add(OperatorCycle([":", "\n"]));
            if (!Equal("]"))
                PrintError(ErrorType.Expected, "]");
            else
                ReadNext();
            return node;
        }

        if (Equal("if"))
        {
            ReadNext();
            node.Add(Compare().Item1);
            var jif_id = _actions.Count;
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.JumpIfFalse));
            ReadNextIfEqualElseThrow("then");
            node.Add(Oper());
            if (Equal("else"))
            {
                _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Jump));
                var j_id = _actions.Count;
                _actions.Insert(jif_id, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 2));
                ReadNext();
                node.Add(Oper());
                _actions.Insert(j_id, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 1));
            }
            else
            {
                _actions.Insert(jif_id, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 1));
            }

            return node;
        }

        if (Equal("while"))
        {
            ReadNext();
            var jId = _actions.Count;
            node.Add(Compare().Item1);
            var jifId = _actions.Count;
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.JumpIfFalse));
            ReadNextIfEqualElseThrow("do");
            node.Add(Oper());
            _actions.Add(new AddressAction(AddressAction.AddressType.Instruction, jId));
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Jump));
            _actions.Insert(jifId, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count));
            return node;
        }

        if (Equal("for"))
        {
            ReadNext();
            var jTo = _actions.Count;
            node.Add(Assign());
            ReadNextIfEqualElseThrow("to");
            node.Add(Compare().Item1);
            var jifFrom = _actions.Count;
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.JumpIfFalse));
            ReadNextIfEqualElseThrow("do");

            node.Add(Oper());
            _actions.Add(new AddressAction(AddressAction.AddressType.Instruction, jTo));
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Jump));
            _actions.Insert(jifFrom, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 1));
            return node;
        }

        if (Equal("read"))
        {
            ReadNext();
            ReadNextIfEqualElseThrow("(");
            node.Add(Identifiers());
            ReadNextIfEqualElseThrow(")");

            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Read));
            return node;
        }

        if (Equal("write"))
        {
            ReadNext();
            ReadNextIfEqualElseThrow("(");
            node.Add(Expression());
            ReadNextIfEqualElseThrow(")");

            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Print));
            return node;
        }

        node.Add(Assign());
        return node;
    }

    private Node<string> OperatorCycle(HashSet<string> separators)
    {
        var node = CreateNode();
        node.Add(Oper());
        while (separators.Contains(_tokenValue))
        {
            ReadNext();
            node.Add(Oper());
        }

        return node;
    }

    private Node<string> Vars()
    {
        var node = CreateNode();
        ReadNextIfEqualElseThrow("var");

        while (true)
        {
            if (Equal("begin"))
                break;
            var temp = Description();
            node.Add(temp.Item1);
            if (!temp.Item2 || !Equal(";"))
                break;
            ReadNext();
        }

        return node;
    }

    private (Node<string>, bool) Description()
    {
        var node = CreateNode();
        if (!Equal("int") && !Equal("float") && !Equal("bool")) return (node, false);
        var kind = _tokenValue switch
        {
            "int" => ValueKind.Integer,
            "float" => ValueKind.Double,
            "bool" => ValueKind.Boolean,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (!_kinds.Add(kind))
            PrintError(ErrorType.Redefinition, _tokenValue);
        do
        {
            ReadNext();
            if (!Equal(TableType.Identifiers))
            {
                PrintError(ErrorType.Expected, "identifier");
                return (node, true);
            }
            else
            {
                if (_variables.ContainsKey(_tokenValue))
                {
                    PrintError(ErrorType.Redefinition, _tokenValue);
                }
                else
                {
                    _variables[_tokenValue] = _values.Count;
                    _values.Add(new UniValue(kind));
                }
            }

            ReadNext();
        } while (Equal(","));

        return (node, true);
    }

    private Node<string> Body()
    {
        var node = CreateNode();
        ReadNextIfEqualElseThrow("begin");
        node.Add(OperatorCycle([";"]));
        ReadNextIfEqualElseThrow("end");
        ReadNextIfEqualElseThrow(".");
        return node;
    }
}