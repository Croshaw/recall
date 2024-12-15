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
        Start();
    }

    public IReadOnlyList<object> Actions => _actions;
    public IReadOnlyDictionary<string, int> Variables => _variables;
    public List<UniValue> Values => _values;

    private void PrintError(ErrorType errorType, string expected)
    {
        if (!HasErrors)
            HasErrors = true;
        _console.CER.WriteLine(Error.GetMesg(errorType, expected, _currentToken.Position));
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

    private bool Value()
    {
        if (Equal("not"))
        {
            ReadNext();
            Value();
            _actions.Add(new UnaryOperatorAction("not"));
            return true;
        }

        if (Equal("true") || Equal("false") || Equal(TableType.Numbers))
        {
            try
            {
                _values.Add(UniValue.Parse(_tokenValue));
                _actions.Add(new AddressAction(AddressAction.AddressType.Value, _values.Count - 1));
                return true;
            }
            catch (Exception ex)
            {
                _console.CER.WriteLine(ex.Message);
            }

            return false;
        }

        if (Equal(TableType.Identifiers))
        {
            if (!_variables.ContainsKey(_tokenValue))
                PrintError(ErrorType.Undeclared, _tokenValue);
            else
                _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
            return true;
        }

        if (Equal("("))
        {
            ReadNext();
            Compare();
            if (!Equal(")"))
                PrintError(ErrorType.Expected, _tokenValue);
            return true;
        }

        return false;
    }

    private bool Compare()
    {
        if (Value())
        {
            ReadNext();
            if (Equal("*") || Equal("/") || Equal("and") || Equal("+") || Equal("-") || Equal("or") || Equal("=") ||
                Equal("<>") || Equal(">") || Equal("<") || Equal(">=") || Equal("<="))
            {
                var @operator = _tokenValue;
                ReadNext();
                if (Compare())
                    _actions.Add(new OperatorAction(@operator));
            }

            return true;
        }

        return false;
    }

    private void Assign()
    {
        if (!Equal(TableType.Identifiers))
        {
            PrintError(ErrorType.Expected, "identifier");
            return;
        }

        if (!_variables.ContainsKey(_tokenValue))
        {
            PrintError(ErrorType.Undeclared, _tokenValue);
            return;
        }

        _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
        ReadNext();
        ReadNextIfEqualElseThrow("as");
        Compare();
        _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Assign));
    }

    private void Identifiers()
    {
        if (!Equal(TableType.Identifiers))
        {
            PrintError(ErrorType.Expected, "identifier");
            return;
        }

        if (!_variables.ContainsKey(_tokenValue))
        {
            PrintError(ErrorType.Undeclared, _tokenValue);
            return;
        }

        _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
        ReadNext();
        while (Equal(","))
        {
            ReadNext();
            if (!Equal(TableType.Identifiers))
            {
                PrintError(ErrorType.Expected, "identifier");
                return;
            }

            if (!_variables.ContainsKey(_tokenValue))
            {
                PrintError(ErrorType.Undeclared, _tokenValue);
                return;
            }

            _actions.Add(new AddressAction(AddressAction.AddressType.Value, _variables[_tokenValue]));
            ReadNext();
        }
    }

    private void Start()
    {
        try
        {
            ReadNextIfEqualElseThrow("program");
            Vars();
            Body();
        }
        catch (Exception ex)
        {
            _console.CER.WriteLine(ex.Message);
        }
    }

    private void Expression()
    {
        Compare();
        while (Equal(","))
        {
            ReadNext();
            Compare();
        }
    }

    private void Oper()
    {
        if (Equal("["))
        {
            ReadNext();
            OperatorCycle([":", "\n"]);
            if (!Equal("]"))
                PrintError(ErrorType.Expected, "]");
            else
                ReadNext();
            return;
        }

        if (Equal("if"))
        {
            ReadNext();
            Compare();
            var jif_id = _actions.Count;
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.JumpIfFalse));
            ReadNextIfEqualElseThrow("then");
            Oper();
            if (Equal("else"))
            {
                _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Jump));
                var j_id = _actions.Count;
                _actions.Insert(jif_id, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 2));
                ReadNext();
                Oper();
                _actions.Insert(j_id, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 1));
            }
            else
            {
                _actions.Insert(jif_id, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 1));
            }

            return;
        }

        if (Equal("while"))
        {
            ReadNext();
            var jId = _actions.Count;
            Compare();
            var jifId = _actions.Count;
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.JumpIfFalse));
            ReadNextIfEqualElseThrow("do");
            Oper();
            _actions.Add(new AddressAction(AddressAction.AddressType.Instruction, jId));
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Jump));
            _actions.Insert(jifId, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count));
            return;
        }

        if (Equal("for"))
        {
            ReadNext();
            var jTo = _actions.Count;
            Assign();
            ReadNextIfEqualElseThrow("to");
            Compare();
            var jifFrom = _actions.Count;
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.JumpIfFalse));
            ReadNextIfEqualElseThrow("do");

            Oper();
            _actions.Add(new AddressAction(AddressAction.AddressType.Instruction, jTo));
            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Jump));
            _actions.Insert(jifFrom, new AddressAction(AddressAction.AddressType.Instruction, _actions.Count + 1));
            return;
        }

        if (Equal("read"))
        {
            ReadNext();
            ReadNextIfEqualElseThrow("(");
            Identifiers();
            ReadNextIfEqualElseThrow(")");

            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Read));
            return;
        }

        if (Equal("write"))
        {
            ReadNext();
            ReadNextIfEqualElseThrow("(");
            Expression();
            ReadNextIfEqualElseThrow(")");

            _actions.Add(new SpecialAction(SpecialAction.SpecialActionType.Print));
            return;
        }

        Assign();
    }

    private void OperatorCycle(HashSet<string> separators)
    {
        Oper();
        while (separators.Contains(_tokenValue))
        {
            ReadNext();
            Oper();
        }
    }

    private void Vars()
    {
        ReadNextIfEqualElseThrow("var");

        do
        {
            if (Equal("begin"))
                return;
            if (!Description())
                break;
            ReadNext();
        } while (Equal(";"));
    }

    private bool Description()
    {
        if (Equal("int") || Equal("float") || Equal("bool"))
        {
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
                    return false;
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

            return true;
        }

        return false;
    }

    private void Body()
    {
        ReadNextIfEqualElseThrow("begin");
        OperatorCycle([";"]);
        ReadNextIfEqualElseThrow("end");
        ReadNextIfEqualElseThrow(".");
    }
}