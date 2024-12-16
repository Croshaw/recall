using Call.Core.Configuration;
using Call.Core.Interpreter;
using Call.Core.Lexing;
using Call.Core.Utilities;
using Console = Call.Core.Configuration.Console;

namespace Call.Core.Syntax;

public class Parser
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

    public Parser(Settings settings, IReadOnlyList<Token> tokens, Console console)
    {
        _settings = settings;
        _tokens = tokens;
        _console = console;
        _kinds = [];
        _actions = [];
        _variables = [];
        _values = [];
        _index = -1;
        ReadNext();
    }

    public IReadOnlyList<object> Actions => _actions;
    public IReadOnlyDictionary<string, int> Variables => _variables;
    public List<UniValue> Values => _values;

    private void PrintError(ErrorType errorType, string expected, Position? pos = null)
    {
        if (!HasErrors)
            HasErrors = true;
        _console.CER.WriteLine(Error.GetMesg(errorType, expected, pos.HasValue ? pos!.Value : _currentToken.Position));
    }

    private bool Equal(string value)
    {
        return _tokenValue == value;
    }

    private bool EqualAny(params ReadOnlySpan<string> values)
    {
        foreach (var value in values)
            if (_tokenValue == value)
                return true;

        return false;
    }

    private bool Equal(TableType tableType)
    {
        return _currentToken.Table == tableType;
    }

    private void ReadNext()
    {
        if (_index + 1 < _tokens.Count)
            _index++;
        else
            throw new EndOfStreamException();
        _currentToken = _tokens[_index];
        _tokenValue = _settings.GetTable(_currentToken.Table)[_currentToken.Id];
    }

    private bool ReadNextIfEqual(string value, Node<string>? node = null)
    {
        var result = false;
        try
        {
            if (Equal(value))
            {
                result = true;
                if (node is not null)
                    node.Add(value);
                ReadNext();
                return true;
            }
        }
        catch (EndOfStreamException e)
        {
        }

        return result;
    }

    private bool ReadNextIfEqual(TableType value, Node<string>? node = null)
    {
        var result = false;
        try
        {
            if (Equal(value))
            {
                result = true;
                if (node is not null)
                    node.Add(_tokenValue);
                ReadNext();
            }
        }
        catch (EndOfStreamException e)
        {
        }

        return result;
    }

    private bool ReadNextIfEqualElseThrow(string value, Node<string>? node = null)
    {
        var result = false;
        try
        {
            if (Equal(value))
            {
                result = true;
                if (node is not null)
                    node.Add(value);
                ReadNext();
            }
            else
            {
                PrintError(ErrorType.Expected, value);
            }
        }
        catch (EndOfStreamException e)
        {
            // PrintError(ErrorType.Expected, value);
        }

        return result;
    }

    private bool ReadNextIfEqualElseThrow(TableType value, Node<string>? node = null)
    {
        var result = false;

        try
        {
            if (Equal(value))
            {
                result = true;
                if (node is not null)
                    node.Add(_tokenValue);
                ReadNext();
                return true;
            }
            else
            {
                PrintError(ErrorType.Expected, value.ToString());
            }
        }
        catch (EndOfStreamException e)
        {
        }

        return result;
    }

    private bool ReadNextIfEqual(Node<string>? node = null, params ReadOnlySpan<string> values)
    {
        var result = false;
        try
        {
            var errorString = "";
            foreach (var value in values)
            {
                if (Equal(value))
                {
                    result = true;
                    if (node is not null)
                        node.Add(value);
                    ReadNext();
                    break;
                }

                errorString += value + " or ";
            }
        }
        catch (EndOfStreamException e)
        {
        }

        return result;
    }

    private bool ReadNextIfEqualElseThrow(Node<string>? node = null, params ReadOnlySpan<string> values)
    {
        var result = false;
        try
        {
            var errorString = "";
            foreach (var value in values)
            {
                if (Equal(value))
                {
                    result = true;
                    if (node is not null)
                        node.Add(value);
                    ReadNext();
                    break;
                }

                errorString += value + " or ";
            }

            PrintError(ErrorType.Expected, errorString.Substring(0, errorString.Length - 4));
        }
        catch (EndOfStreamException e)
        {
        }

        return result;
    }

    private void AddAction(SpecialAction.SpecialActionType specialActionType)
    {
        _actions.Add(new SpecialAction(specialActionType));
    }

    private void AddAction(string @operator)
    {
        _actions.Add(@operator == "not" ? new UnaryOperatorAction(@operator) : new OperatorAction(@operator));
    }

    private void AddAction(AddressAction.AddressType addressType, int address)
    {
        _actions.Add(new AddressAction(addressType, address));
    }

    private void InsertAction(int position, AddressAction.AddressType addressType, int address)
    {
        _actions.Insert(position, new AddressAction(addressType, address));
    }

    private void AddValue()
    {
        AddValue(_tokenValue);
    }

    private void AddValue(string value)
    {
        _values.Add(UniValue.Parse(value));
        AddAction(AddressAction.AddressType.Value, _values.Count - 1);
    }

    public Node<string> Pr()
    {
        // var node = ParserUtils.CreateNode();
        try
        {
            ReadNextIfEqualElseThrow("program");
            Vars();
            return Body();
        }
        catch (EndOfStreamException e)
        {
            return null;
        }
    }

    private void Vars()
    {
        ReadNextIfEqualElseThrow("var");
        while (true)
        {
            if (Equal("begin"))
                break;
            if (!Descr())
                break;
            ReadNext();
        }
    }

    private bool Descr()
    {
        if (!EqualAny("int", "float", "bool"))
        {
            PrintError(ErrorType.Expected, "int or float or bool");
            return false;
        }

        var kind = _tokenValue switch
        {
            "int" => ValueKind.Integer,
            "float" => ValueKind.Double,
            "bool" => ValueKind.Boolean,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (!_kinds.Add(kind))
            PrintError(ErrorType.Redefinition, _tokenValue);
        ReadNext();
        while (true)
        {
            if (!Equal(TableType.Identifiers))
                break;

            if (_variables.ContainsKey(_tokenValue))
            {
                PrintError(ErrorType.Redefinition, _tokenValue);
            }
            else
            {
                _variables[_tokenValue] = _values.Count;
                _values.Add(new UniValue(kind));
            }

            ReadNext();
            if (Equal(";"))
                break;
            ReadNextIfEqualElseThrow(",");
        }

        return true;
    }

    private Node<string> Body()
    {
        var node = ParserUtils.CreateNode();
        ReadNextIfEqualElseThrow("begin");
        node.Add(Oper1(";"));
        ReadNextIfEqualElseThrow("end");
        ReadNextIfEqualElseThrow(".");
        return node;
    }

    private Node<string> Oper1(params ReadOnlySpan<string> separators)
    {
        var node = ParserUtils.CreateNode();
        do
        {
            node.Add(Oper());
            if (!separators.Contains(_tokenValue))
                break;
            ReadNext();
        } while (true);

        return node;
    }

    private Node<string> Oper()
    {
        var node = ParserUtils.CreateNode();

        switch (_tokenValue)
        {
            case "[":
                Compound(node);
                break;
            case "if":
                If(node);
                break;
            case "while":
                While(node);
                break;
            case "for":
                For(node);
                break;
            case "read":
                Read(node);
                break;
            case "write":
                Write(node);
                break;
            default:
                node.Add(Assign());
                break;
        }

        return node;
    }

    private void Compound(Node<string> node)
    {
        ReadNextIfEqualElseThrow("[");
        node.Add(Oper1(":", "\n"));
        ReadNextIfEqualElseThrow("]");
    }

    private void If(Node<string> node)
    {
        ReadNextIfEqualElseThrow("if", node);
        node.Add(Compare());

        var jif_id = _actions.Count;
        AddAction(SpecialAction.SpecialActionType.JumpIfFalse);

        ReadNextIfEqualElseThrow("then", node);
        node.Add(Oper());
        if (ReadNextIfEqual("else", node))
        {
            AddAction(SpecialAction.SpecialActionType.Jump);
            var j_id = _actions.Count;
            InsertAction(jif_id, AddressAction.AddressType.Instruction, _actions.Count + 2);
            node.Add(Oper());
            InsertAction(j_id, AddressAction.AddressType.Instruction, _actions.Count + 1);
        }
        else
        {
            InsertAction(jif_id, AddressAction.AddressType.Instruction, _actions.Count + 1);
        }
    }

    private void While(Node<string> node)
    {
        ReadNextIfEqualElseThrow("while", node);
        var jId = _actions.Count;
        node.Add(Compare());
        var jifId = _actions.Count;
        AddAction(SpecialAction.SpecialActionType.JumpIfFalse);
        ReadNextIfEqualElseThrow("do", node);
        node.Add(Oper());
        AddAction(AddressAction.AddressType.Instruction, jId);
        AddAction(SpecialAction.SpecialActionType.Jump);
        InsertAction(jifId, AddressAction.AddressType.Instruction, _actions.Count + 1);
    }

    private void For(Node<string> node)
    {
        ReadNextIfEqualElseThrow("for", node);
        var jTo = _actions.Count;
        node.Add(Assign());
        ReadNextIfEqualElseThrow("to", node);
        node.Add(Compare());
        var jifFrom = _actions.Count;
        AddAction(SpecialAction.SpecialActionType.JumpIfFalse);
        ReadNextIfEqualElseThrow("do", node);
        node.Add(Oper());
        AddAction(AddressAction.AddressType.Instruction, jTo);
        AddAction(SpecialAction.SpecialActionType.Jump);
        InsertAction(jifFrom, AddressAction.AddressType.Instruction, _actions.Count + 1);
    }

    private void Read(Node<string> node)
    {
        ReadNextIfEqualElseThrow("read", node);
        ReadNextIfEqualElseThrow("(", node);
        node.Add(Id1());
        ReadNextIfEqualElseThrow(")", node);
        AddAction(SpecialAction.SpecialActionType.Read);
    }

    private void Write(Node<string> node)
    {
        var _node = ParserUtils.CreateNode();
        node.Add(_node);
        ReadNextIfEqualElseThrow("write", _node);
        ReadNextIfEqualElseThrow("(", _node);
        _node.Add(Expr());
        ReadNextIfEqualElseThrow(")", _node);
        AddAction(SpecialAction.SpecialActionType.Print);
    }

    public Node<string> Id1()
    {
        var node = ParserUtils.CreateNode();
        while (true)
        {
            if (!Id(node))
                break;
            if (!ReadNextIfEqual(","))
                break;
        }

        return node;
    }

    public bool Id(Node<string> node)
    {
        var idNode = ParserUtils.CreateNode();
        var variable = _tokenValue;
        var pos = _currentToken.Position;
        if (!ReadNextIfEqual(TableType.Identifiers, idNode))
            return false;
        node.Add(idNode);
        if (!_variables.ContainsKey(variable))
            PrintError(ErrorType.Undeclared, variable, pos);
        else
            AddAction(AddressAction.AddressType.Value, _variables[variable]);
        return true;
    }

    public bool Number(Node<string> node)
    {
        var hzNode = ParserUtils.CreateNode();
        var token = _tokenValue;
        var result = ReadNextIfEqual(TableType.Numbers, hzNode);
        if (!result) return result;
        AddValue(token);
        node.Add(hzNode);

        return result;
    }

    public bool Log(Node<string> node)
    {
        var hzNode = ParserUtils.CreateNode();
        var token = _tokenValue;
        var result = ReadNextIfEqual(hzNode, "true", "false");
        if (!result) return result;
        AddValue(token);
        node.Add(hzNode);
        return result;
    }

    public Node<string> Assign()
    {
        var node = ParserUtils.CreateNode();
        Id(node);
        ReadNextIfEqualElseThrow("as", node);
        node.Add(Compare());
        AddAction(SpecialAction.SpecialActionType.Assign);
        return node;
    }

    public Node<string> Expr()
    {
        var node = ParserUtils.CreateNode();
        while (true)
        {
            node.Add(Compare());
            if (!ReadNextIfEqual(","))
                break;
        }

        return node;
    }

    public Node<string> Compare()
    {
        var node = ParserUtils.CreateNode();
        node.Add(Add());
        var token = _tokenValue;
        if (!ReadNextIfEqual(node, "=", ">", "<", "<>", "<=", ">=")) return node;
        node.Add(Compare());
        AddAction(token);
        return node;
    }

    public Node<string> Add()
    {
        var node = ParserUtils.CreateNode();
        node.Add(Mult());
        var token = _tokenValue;
        if (!ReadNextIfEqual(node, "+", "-", "or")) return node;
        node.Add(Add());
        AddAction(token);
        return node;
    }

    public Node<string> Mult()
    {
        var node = ParserUtils.CreateNode();
        node.Add(Fact());
        var token = _tokenValue;
        if (!ReadNextIfEqual(node, "*", "/", "and")) return node;
        node.Add(Mult());
        AddAction(token);
        return node;
    }

    public Node<string> Fact()
    {
        var node = ParserUtils.CreateNode();
        if (Id(node))
            return node;
        if (Number(node))
            return node;
        if (Log(node))
            return node;
        if (ReadNextIfEqual("not", node))
        {
            AddAction("not");
            node.Add(Fact());
            return node;
        }

        if (ReadNextIfEqual("(", node))
        {
            node.Add(Compare());
            ReadNextIfEqualElseThrow(")", node);
            return node;
        }

        PrintError(ErrorType.Expected, "id or number or log or compare");
        return node;
    }
}