using Call.Core.Configuration;
using Call.Core.Utilities;
using Console = Call.Core.Configuration.Console;

namespace Call.Core.Lexing;

using Tokens = List<Token>;
using ReadOnlyTokens = IReadOnlyList<Token>;

public class Lexer
{
    private readonly Console _console;
    private readonly Settings _settings;
    private readonly string _source;
    private readonly Tokens _tokens;

    private char _cur;
    private int _index;
    private string _lexem;

    private Position _position;
    private int _start;
    private State _state;
    public bool HasErrors { get; private set; }

    public Lexer(string source, Settings settings, Console console)
    {
        _source = source;
        _settings = settings;
        _console = console;
        _position = new Position(1, 1, 0, 0);
        _tokens = new Tokens();
        Parse();
    }

    public ReadOnlyTokens Tokens => _tokens.AsReadOnly();

    private bool IsNumber()
    {
        return char.IsDigit(_cur);
    }

    private bool IsLetter()
    {
        return char.IsLetter(_cur);
    }

    private bool IsSep()
    {
        return _settings.IndexOf(TableType.Separators, _cur.ToString()) != -1;
    }

    private void Reset()
    {
        _lexem = string.Empty;
        _state = State.Begin;
        _start = _index - 1;
    }

    private void Read()
    {
        _cur = _source[_index];
        _index++;
    }

    private void PrintError()
    {
        if (!HasErrors)
            HasErrors = true;
        _console.CER.WriteLine(Error.GetMesg(ErrorType.Unknown, _source.Substring(_start, _index - _start), _position));
    }

    private void CreateToken()
    {
        if (string.IsNullOrEmpty(_lexem))
            return;
        _position = new Position(_position.Row, _start - _position.Absolute + _position.Column,
            Math.Max(_index - _start - 1, 1),
            _start);
        if (_state.IsNum())
        {
            if (!_state.HasFlag(State.NumEnd))
            {
                if (_state.HasFlag(State.Dec))
                    _lexem += 'd';
                else if (_state.HasAnyFlags(State.Bin, State.Oct, State.Hex))
                    _state = State.Error;
            }

            _lexem = _lexem.TrimStart('0');
        }

        if (_state == State.Error)
        {
            PrintError();
            return;
        }

        var tableType = _state == State.Word ? TableType.Identifiers :
            _state == State.Sep ? TableType.Separators : TableType.Numbers;
        var id = -1;
        if (tableType == TableType.Identifiers)
        {
            id = _settings.IndexOf(TableType.Reserved, _lexem);
            if (id != -1)
                tableType = TableType.Reserved;
        }

        if (tableType != TableType.Reserved)
            id = _settings.Add(tableType, _lexem);

        if (id < 0)
            throw new Exception($"Invalid table type: {tableType}"); // Переделать
        _tokens.Add(new Token(tableType, id, _position));
    }

    private State BeginStateProcessing()
    {
        if (IsNumber() || _cur == '.')
            return NumberStateProcessing();
        if (IsLetter())
            return WordStateProcessing();
        if (IsSep())
            return SepStateProcessing();
        return State.Error;
    }

    private State WordStateProcessing()
    {
        if (IsSep())
        {
            CreateToken();
            Reset();
            return State.Sep;
        }

        return State.Word;
    }

    private State SepStateProcessing()
    {
        if (IsNumber())
        {
            CreateToken();
            Reset();
            return NumberStateProcessing();
        }

        if (IsLetter())
        {
            CreateToken();
            Reset();
            return WordStateProcessing();
        }

        if (!string.IsNullOrEmpty(_lexem) && _settings.IndexOf(TableType.Separators, _lexem + _cur) == -1)
        {
            CreateToken();
            Reset();
        }

        return State.Sep;
    }

    private State NumberStateProcessing()
    {
        if (IsSep())
        {
            CreateToken();
            Reset();
            return State.Sep;
        }

        if (_state.HasFlag(State.NumEnd))
            _state &= ~State.NumEnd;
        if (IsLetter())
        {
            if (!_state.HasFlag(State.Por))
            {
                _cur = char.ToLower(_cur);
                switch (_cur)
                {
                    case 'a':
                    case 'c':
                    case 'f':
                        if (_state.HasFlag(State.Hex))
                            return State.Hex;
                        break;
                    case 'b':
                        if (_state.HasFlag(State.Hex))
                        {
                            if (_state.HasFlag(State.Bin))
                                return State.Bin | State.NumEnd;
                            return State.Hex;
                        }

                        if (_state.HasFlag(State.Bin))
                            return State.Hex;

                        break;
                    case 'd':
                        if (_state.HasFlag(State.Hex))
                        {
                            if (_state.HasFlag(State.Dec))
                                return State.Dec | State.NumEnd;
                            return State.Hex;
                        }

                        if (_state.HasFlag(State.Dec))
                            return State.Hex;

                        break;
                    case 'e':
                        if (_state.HasFlag(State.Exp))
                        {
                            if (_state.HasFlag(State.Hex))
                                return State.Hex;
                            break;
                        }

                        if (_state.HasFlag(State.Dec))
                            return State.Exp | State.Hex;
                        if (_state.HasFlag(State.Float))
                            return State.Exp;
                        return _state;
                    case 'h':
                        if (_state.HasAnyFlags(State.Bin, State.Oct, State.Dec, State.Hex))
                            return State.Hex | State.NumEnd;
                        break;
                    case 'o':
                        if (_state.HasFlag(State.Oct))
                            return State.Oct | State.NumEnd;
                        break;
                }
            }
        }
        else if (_cur == '.')
        {
            if (!_state.HasFlag(State.Float) && !_state.HasFlag(State.Por) && !_state.HasFlag(State.Exp) &&
                (_state == State.Begin || _state.HasFlag(State.Dec)))
                return State.Float;
        }
        else if (_cur == '+' || _cur == '-')
        {
            if (!_state.HasFlag(State.Por) && _state.HasFlag(State.Exp))
                return State.Exp | State.Por;
        }
        else
        {
            if (_state == State.Begin)
            {
                var state = State.Dec | State.Hex;
                if (_cur < '8')
                    state |= State.Oct;
                if (_cur < '2')
                    state |= State.Bin;
                return state;
            }

            if (_state.HasFlag(State.Exp) && !_state.HasFlag(State.Por))
            {
                _lexem += '+';
                return State.Exp | State.Por;
            }

            if (_cur > '7' && _state.HasAnyFlags(State.Bin, State.Oct))
                return State.Dec | State.Hex;
            if (_cur > '1' && _state.HasFlag(State.Bin))
                return State.Oct | State.Dec | State.Hex;
            return _state;
        }

        return State.Error;
    }

    private State GetState()
    {
        switch (_state)
        {
            case State.Begin:
                _start = _index - 1;
                return BeginStateProcessing();
            case State.Word:
                return WordStateProcessing();
            case State.Sep:
                return SepStateProcessing();
            default:
                return _state.IsNum() ? NumberStateProcessing() : State.Error;
        }
    }

    private void Parse()
    {
        Reset();
        while (_index < _source.Length)
        {
            Read();
            switch (_cur)
            {
                case ' ':
                    CreateToken();
                    Reset();
                    break;
                default:
                    if (!_state.HasFlag(State.Error))
                        _state = GetState();
                    _lexem += _cur;
                    if (_cur == '\n')
                        _position = new Position(_position.Row + 1, 1, 0, _index + 1);
                    break;
            }
        }

        CreateToken();
    }

    public override string ToString()
    {
        var result = "";
        var lastRow = 1;
        var lastPos = 0;
        foreach (var token in _tokens)
        {
            var value = _settings.GetTable(token.Table)[token.Id];
            var curPos = token.Position.Absolute;
            var length = token.Position.Length;
            var curRow = token.Position.Row;
            result += string.Join("", Enumerable.Repeat('\n', curRow - lastRow));
            result += string.Join("", Enumerable.Repeat(' ', curPos - lastPos - (curRow - lastRow)));
            result += value;
            lastPos = curPos + length;
            lastRow = curRow;
        }

        return result;
    }
}