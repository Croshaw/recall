namespace Call.Core.Interpreter;

public class Interpretator
{
    private readonly IReadOnlyList<object> _actions;
    private readonly IReadOnlyDictionary<string, int> _variables;
    private readonly List<UniValue> _values;
    private readonly Call.Core.Configuration.Console _console;
    private readonly Stack<AddressAction> _stack;

    private Interpretator(IReadOnlyList<object> actions, IReadOnlyDictionary<string, int> variables,
        List<UniValue> values, Call.Core.Configuration.Console console)
    {
        _actions = actions;
        _variables = variables;
        _values = values;
        _console = console;
        _stack = [];
    }

    private void Run()
    {
        for (var i = 0; i < _actions.Count; i++)
            try
            {
                var action = _actions[i];
                if (action is AddressAction addressAction)
                {
                    _stack.Push(addressAction);
                }
                else if (action is OperatorAction operatorAction)
                {
                    var second = _stack.Pop();
                    var first = _stack.Pop();
                    _stack.Push(new AddressAction(AddressAction.AddressType.Value, _values.Count));
                    _values.Add(operatorAction.Evaluate(_values[first.Address], _values[second.Address]));
                }
                else if (action is UnaryOperatorAction unaryOperatorAction)
                {
                    var first = _stack.Pop();
                    _stack.Push(new AddressAction(AddressAction.AddressType.Value, _values.Count));
                    _values.Add(unaryOperatorAction.Evaluate(_values[first.Address]));
                }
                else if (action is SpecialAction specialAction)
                {
                    switch (specialAction.Type)
                    {
                        case SpecialAction.SpecialActionType.Assign:
                        {
                            var second = _stack.Pop();
                            var first = _stack.Pop();
                            _values[first.Address] = _values[second.Address];
                            break;
                        }
                        case SpecialAction.SpecialActionType.JumpIfFalse:
                        {
                            var address = _stack.Pop();
                            var valueAddress = _stack.Pop();
                            if (!_values[valueAddress.Address].GetBoolValue())
                                i = address.Address;
                            break;
                        }
                        case SpecialAction.SpecialActionType.JumpIfTrue:
                        {
                            var address = _stack.Pop();
                            var valueAddress = _stack.Pop();
                            if (_values[valueAddress.Address].GetBoolValue())
                                i = address.Address;
                            break;
                        }
                        case SpecialAction.SpecialActionType.Jump:
                        {
                            var address = _stack.Pop();
                            i = address.Address;
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

                            _console.COUT.Write('\n');

                            _stack.Clear();
                            break;
                        case SpecialAction.SpecialActionType.Read:
                            while (_stack.Count > 0)
                            {
                                var address = _stack.Pop();
                                _values[address.Address] = UniValue.Parse(_console.CIN.ReadLine());
                            }

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _console.CER.WriteLine(ex.Message);
            }
    }

    public static void Execute(IReadOnlyList<object> actions, IReadOnlyDictionary<string, int> variables,
        List<UniValue> values, Call.Core.Configuration.Console console)
    {
        new Interpretator(actions, variables, values, console).Run();
    }
}