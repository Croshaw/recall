using Call.Core.Configuration;
using Call.Core.Utilities;

namespace Call.Core.Lexing;

public readonly struct Token
{
    public TableType Table { get; }
    public int Id { get; }
    public Position Position { get; }

    public Token(TableType table, int id, Position position)
    {
        Table = table;
        Id = id;
        Position = position;
    }
}