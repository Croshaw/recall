using System.Collections.ObjectModel;

namespace Call.Core.Configuration;

using Table = List<string>;
using ReadOnlyTable = ReadOnlyCollection<string>;

public class Settings
{
    public static readonly ReadonlySettings Default = new([
        "program", "var", "int", "float", "bool", "begin", "end", "if", "then", "else", "while", "do", "for", "to",
        "read", "write", "true", "false", "or", "and", "as", "not"
    ], [
        ".", ";", ",", ":", "(", ")", "[", "]", "+", "-", "*", "/", "=", ">", "<", "<=", ">=", "<>", "\n"
    ]);

    public class ReadonlySettings
    {
        private IEnumerable<string> _reserved;
        private IEnumerable<string> _separators;

        public ReadonlySettings(IEnumerable<string> reserved, IEnumerable<string> separators)
        {
            _reserved = reserved;
            _separators = separators;
        }

        public static implicit operator Settings(ReadonlySettings value)
        {
            return new Settings(value._reserved, value._separators);
        }
    }

    private readonly Dictionary<TableType, Table> _tables;
    public IReadOnlyDictionary<TableType, Table> Tables => _tables.AsReadOnly();

    public Settings(IEnumerable<string> reserved, IEnumerable<string> separators)
    {
        _tables = new Dictionary<TableType, Table>();
        _tables[TableType.Reserved] = reserved.ToList();
        _tables[TableType.Separators] = separators.ToList();
        _tables[TableType.Numbers] = [];
        _tables[TableType.Identifiers] = [];
    }

    public void Reset()
    {
        _tables[TableType.Numbers].Clear();
        _tables[TableType.Identifiers].Clear();
    }

    public ReadOnlyTable GetTable(TableType type)
    {
        return _tables[type].AsReadOnly();
    }

    public int IndexOf(TableType tableType, string value)
    {
        var index = -1;
        if (_tables.TryGetValue(tableType, out var table))
            index = table.IndexOf(value);
        return index;
    }

    public int Add(TableType tableType, string value)
    {
        var index = IndexOf(tableType, value);
        switch (tableType)
        {
            case TableType.Reserved:
            case TableType.Separators:
                return index;
            default:
                if (index == -1)
                    if (_tables.TryGetValue(tableType, out var table))
                    {
                        table.Add(value);
                        index = table.Count - 1;
                    }

                return index;
        }
    }
}