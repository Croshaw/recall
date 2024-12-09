namespace Call.Core.Lexing;

[Flags]
public enum State
{
    Begin = 0,
    Word = 1 << 0,
    Sep = 1 << 1,
    Error = 1 << 2,
    Bin = 1 << 3,
    Oct = 1 << 4,
    Dec = 1 << 5,
    Hex = 1 << 6,
    Float = 1 << 7,
    Exp = 1 << 8,
    Por = 1 << 9,
    NumEnd = 1 << 10,
}

public static class StateExtensions
{
    public static bool IsNum(this State state)
    {
        const State numberStates = State.Bin | State.Oct | State.Dec | State.Hex | State.Float | State.Exp | State.Por | State.NumEnd;
        return (state & numberStates) == state && state != State.Begin;
    }

    public static bool HasAnyFlags(this State state, params State[] flags)
    {
        foreach (var flag in flags)
        {
            if (state.HasFlag(flag))
                return true;
        }
        return false;
    }
    public static bool HasFlags(this State state, params State[] flags)
    {
        foreach (var flag in flags)
        {
            if (!state.HasFlag(flag))
                return false;
        }
        return true;
    }
}