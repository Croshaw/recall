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

    public object GetValue()
    {
        return Kind switch
        {
            ValueKind.Double => _raw,
            ValueKind.Integer => (int)_raw,
            ValueKind.Boolean => _raw >= 1.0
        };
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

    public static UniValue Parse(string value)
    {
        if (value == "true")
            return true;
        if (value == "false")
            return false;
        var last = value.Last();
        switch (last)
        {
            case 'd':
                return new UniValue(int.Parse(value.Substring(0, value.Length - 1)));
            case 'h':
                return new UniValue(Convert.ToInt32(value.Substring(0, value.Length - 1), 16));
            case 'b':
                return new UniValue(Convert.ToInt32(value.Substring(0, value.Length - 1), 2));
            case 'o':
                return new UniValue(Convert.ToInt32(value.Substring(0, value.Length - 1), 8));
            default:
                return new UniValue(double.Parse(value));
        }
    }
}