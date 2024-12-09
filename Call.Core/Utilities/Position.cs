namespace Call.Core.Utilities;

public readonly struct Position
{
    public int Row { get; }
    public int Column { get; }
    public int Length { get; }
    public int Absolute { get; }

    public Position(int row, int column, int length, int absolute)
    {
        Row = row;
        Column = column;
        Length = length;
        Absolute = absolute;
    }

    public string ToShortString()
    {
        return $"{Row}:{Column}";
    }
}