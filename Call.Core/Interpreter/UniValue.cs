using System.Globalization;

namespace Call.Core.Interpreter;

public enum ValueKind
{
    Double,
    Integer,
    Boolean
}

public readonly struct UniValue
{
    private readonly double _raw;
    public ValueKind Kind { get; }

    public UniValue(ValueKind kind)
    {
        Kind = kind;
        _raw = 0;
    }

    public UniValue(double raw)
    {
        Kind = ValueKind.Double;
        _raw = raw;
    }

    public UniValue(int raw)
    {
        Kind = ValueKind.Integer;
        _raw = raw;
    }

    public UniValue(bool raw)
    {
        Kind = ValueKind.Boolean;
        _raw = raw ? 1 : 0;
    }

    public UniValue(double raw, ValueKind kind)
    {
        _raw = raw;
        Kind = kind;
    }

    public UniValue(int raw, ValueKind kind)
    {
        _raw = raw;
        Kind = kind;
    }

    public UniValue(bool raw, ValueKind kind)
    {
        Kind = kind;
        _raw = raw ? 1 : 0;
    }

    public static UniValue Assign(UniValue left, UniValue right)
    {
        return new UniValue(right._raw, left.Kind);
    }

    public object GetValue()
    {
        return Kind switch
        {
            ValueKind.Double => _raw,
            ValueKind.Integer => (int)_raw,
            ValueKind.Boolean => _raw >= 1.0
        };
    }

    public override string ToString()
    {
        return GetValue().ToString()!;
    }

    public int GetIntValue()
    {
        return (int)_raw;
    }

    public bool GetBoolValue()
    {
        return _raw != 0;
    }

    public double GetDoubleValue()
    {
        return _raw;
    }

    public static UniValue operator +(UniValue left, UniValue right)
    {
        var kind = (ValueKind)Math.Min((short)left.Kind, (short)right.Kind);

        return new UniValue(
            (left.Kind == ValueKind.Double ? left.GetDoubleValue() : left.GetIntValue())
            + (right.Kind == ValueKind.Double ? right.GetDoubleValue() : right.GetIntValue()),
            kind);
    }

    public static UniValue operator -(UniValue left, UniValue right)
    {
        var kind = (ValueKind)Math.Min((short)left.Kind, (short)right.Kind);
        return new UniValue(
            (left.Kind == ValueKind.Double ? left.GetDoubleValue() : left.GetIntValue())
            - (right.Kind == ValueKind.Double ? right.GetDoubleValue() : right.GetIntValue()),
            kind);
    }

    public static UniValue operator *(UniValue left, UniValue right)
    {
        var kind = (ValueKind)Math.Min((short)left.Kind, (short)right.Kind);
        return new UniValue(
            (left.Kind == ValueKind.Double
                ? left.GetDoubleValue()
                : left.GetIntValue()) * (right.Kind == ValueKind.Double ? right.GetDoubleValue() : right.GetIntValue()),
            kind);
    }

    public static UniValue operator /(UniValue left, UniValue right)
    {
        if (right._raw == 0)
            throw new DivideByZeroException();
        var kind = (ValueKind)Math.Min((short)left.Kind, (short)right.Kind);
        return new UniValue(
            (left.Kind == ValueKind.Double
                ? left.GetDoubleValue()
                : left.GetIntValue()) / (right.Kind == ValueKind.Double ? right.GetDoubleValue() : right.GetIntValue()),
            kind);
    }

    public static bool operator ==(UniValue left, UniValue right)
    {
        return left.Kind == right.Kind && (left.Kind == ValueKind.Double
            ? left.GetDoubleValue()
            : left.GetIntValue()) == (right.Kind == ValueKind.Double
            ? right.GetDoubleValue()
            : right.GetIntValue());
    }

    public static bool operator !=(UniValue left, UniValue right)
    {
        return !(left == right);
    }

    public static bool operator >(UniValue left, UniValue right)
    {
        return (left.Kind == ValueKind.Double
            ? left.GetDoubleValue()
            : left.GetIntValue()) > (right.Kind == ValueKind.Double
            ? right.GetDoubleValue()
            : right.GetIntValue());
    }

    public static bool operator >=(UniValue left, UniValue right)
    {
        return (left.Kind == ValueKind.Double
            ? left.GetDoubleValue()
            : left.GetIntValue()) > (right.Kind == ValueKind.Double
            ? right.GetDoubleValue()
            : right.GetIntValue());
    }

    public static bool operator <(UniValue left, UniValue right)
    {
        return (left.Kind == ValueKind.Double
            ? left.GetDoubleValue()
            : left.GetIntValue()) < (right.Kind == ValueKind.Double
            ? right.GetDoubleValue()
            : right.GetIntValue());
    }

    public static bool operator <=(UniValue left, UniValue right)
    {
        return (left.Kind == ValueKind.Double
            ? left.GetDoubleValue()
            : left.GetIntValue()) <= (right.Kind == ValueKind.Double
            ? right.GetDoubleValue()
            : right.GetIntValue());
    }

    public override bool Equals(object? obj)
    {
        return obj is UniValue other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_raw, Kind);
    }


    public static implicit operator UniValue(bool value)
    {
        return new UniValue(value);
    }

    public static implicit operator bool(UniValue value)
    {
        return value.GetBoolValue();
    }

    public static bool TryParse(string value, out UniValue result)
    {
        try
        {
            result = Parse(value);
            return true;
        }
        catch (Exception ex)
        {
            result = new UniValue(0);
        }

        return false;
    }

    public static UniValue Parse(string value)
    {
        if (value == "true")
            return true;
        if (value == "false")
            return false;
        var last = value.Last();
        var number = char.IsLetter(value.Last()) ? value[..^1] : value;
        if (string.IsNullOrEmpty(number) || (number.Length == 1 && char.IsLetter(number[0])))
            number = "0";
        switch (last)
        {
            case 'd':
                return new UniValue(int.Parse(number));
            case 'h':
                return new UniValue(Convert.ToInt32(number, 16));
            case 'b':
                return new UniValue(Convert.ToInt32(number, 2));
            case 'o':
                return new UniValue(Convert.ToInt32(number, 8));
            default:
                return new UniValue(double.Parse(number));
        }
    }
}