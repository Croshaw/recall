namespace Call.Core.Interpreter;

public class UnaryOperatorAction
{
    private readonly Func<UniValue, UniValue> _function;
    public string Operator { get; }

    public UnaryOperatorAction(string @operator)
    {
        Operator = @operator;
        switch (@operator)
        {
            case "not":
                _function = value => !value;
                break;
        }
    }

    public UniValue Evaluate(UniValue value)
    {
        return _function(value);
    }
}

public class OperatorAction
{
    private readonly Func<UniValue, UniValue, UniValue> _function = null!;
    public string Operator { get; }

    public OperatorAction(string @operator)
    {
        Operator = @operator;
        _function = @operator switch
        {
            "+" => (a, b) => a + b,
            "-" => (a, b) => a - b,
            "or" => (a, b) => a || b,
            "*" => (a, b) => a * b,
            "/" => (a, b) => a / b,
            "and" => (a, b) => a && b,
            ">" => (a, b) => a > b,
            "<" => (a, b) => a < b,
            "=" => (a, b) => a == b,
            "<=" => (a, b) => a <= b,
            ">=" => (a, b) => a >= b,
            "<>" => (a, b) => a != b,
            _ => throw new NotImplementedException()
        };
    }

    public UniValue Evaluate(UniValue x, UniValue y)
    {
        return _function(x, y);
    }
}

public class AddressAction
{
    public enum AddressType
    {
        Value,
        Instruction
    }

    public AddressType Type { get; }
    public int Address { get; }

    public AddressAction(AddressType type, int address)
    {
        Type = type;
        Address = address;
    }
}

public class SpecialAction
{
    public enum SpecialActionType
    {
        JumpIfFalse,
        JumpIfTrue,
        Jump,
        Print,
        Read,
        Assign,
        None
    }

    public SpecialActionType Type { get; }

    public SpecialAction(SpecialActionType type)
    {
        Type = type;
    }
}