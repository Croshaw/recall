using Call.Core.Utilities;

namespace Call.Core.Interpreter;

public class Interpretator
{
    private readonly IReadOnlyList<object> _actions;
    private readonly IReadOnlyDictionary<string, int> _variables;
    private readonly List<UniValue> _values;
    private readonly Call.Core.Configuration.Console _console;
    private readonly Stack<AddressAction> _stack;
    private readonly CancellationToken _cancellationToken;

    private Interpretator(IReadOnlyList<object> actions, IReadOnlyDictionary<string, int> variables,
        List<UniValue> values, Call.Core.Configuration.Console console, CancellationToken? cancellationToken = null)
    {
        _actions = actions;
        _variables = variables;
        _values = values;
        _console = console;
        _stack = [];
        _cancellationToken = cancellationToken ?? CancellationToken.None;
    }

    private void Run()
    {
        for (var i = 0; i < _actions.Count; i++)
            try
            {
                if (_cancellationToken.IsCancellationRequested)
                    return;
                var action = _actions[i];
                switch (action)
                {
                    case AddressAction addressAction:
                        _stack.Push(addressAction);
                        break;
                    case OperatorAction operatorAction:
                    {
                        var second = _stack.Pop();
                        var first = _stack.Pop();
                        _stack.Push(new AddressAction(AddressAction.AddressType.Value, _values.Count));
                        _values.Add(operatorAction.Evaluate(_values[first.Address], _values[second.Address]));
                        break;
                    }
                    case UnaryOperatorAction unaryOperatorAction:
                    {
                        var first = _stack.Pop();
                        _stack.Push(new AddressAction(AddressAction.AddressType.Value, _values.Count));
                        _values.Add(unaryOperatorAction.Evaluate(_values[first.Address]));
                        break;
                    }
                    case SpecialAction specialAction:
                        switch (specialAction.Type)
                        {
                            case SpecialAction.SpecialActionType.Assign:
                            {
                                var second = _stack.Pop();
                                var first = _stack.Pop();
                                _values[first.Address] =
                                    UniValue.Assign(_values[first.Address], _values[second.Address]);
                                break;
                            }
                            case SpecialAction.SpecialActionType.JumpIfFalse:
                            {
                                var address = _stack.Pop();
                                var valueAddress = _stack.Pop();
                                if (!_values[valueAddress.Address].GetBoolValue())
                                    i = address.Address - 1;
                                break;
                            }
                            case SpecialAction.SpecialActionType.JumpIfTrue:
                            {
                                var address = _stack.Pop();
                                var valueAddress = _stack.Pop();
                                if (_values[valueAddress.Address].GetBoolValue())
                                    i = address.Address - 1;
                                break;
                            }
                            case SpecialAction.SpecialActionType.Jump:
                            {
                                var address = _stack.Pop();
                                i = address.Address - 1;
                                break;
                            }
                            case SpecialAction.SpecialActionType.Print:
                                var temp = _stack.ToArray();
                                for (var j = temp.Length - 1; j >= 0; j--)
                                {
                                    var address = temp[j];
                                    _console.COUT.Write(_values[address.Address].GetValue().ToString());
                                    if (j != 0)
                                        _console.COUT.Write(" ");
                                }

                                _console.COUT.WriteLine();

                                _stack.Clear();
                                break;
                            case SpecialAction.SpecialActionType.Read:
                                temp = _stack.ToArray();
                                for (var j = temp.Length - 1; j >= 0; j--)
                                {
                                    var line = _console.CIN.ReadLine();
                                    var numbers = line.Split(" ").Take(temp.Length).ToArray();
                                    for (var b = 0; b < numbers.Length; b++)
                                    {
                                        var address = temp[j];
                                        var number = numbers[b].Trim(' ');
                                        if (!UniValue.TryParse(number, out var value, out _)) continue;
                                        _values[address.Address] = UniValue.Assign(_values[address.Address], value);
                                        j--;
                                    }

                                    j++;
                                }

                                _stack.Clear();
                                break;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                _console.CER.WriteLine(ex.Message);
            }
    }

    public static void Execute(IReadOnlyList<object> actions, IReadOnlyDictionary<string, int> variables,
        List<UniValue> values, Call.Core.Configuration.Console console, CancellationToken? cancellationToken = null)
    {
        new Interpretator(actions, variables, values, console, cancellationToken).Run();
    }
}