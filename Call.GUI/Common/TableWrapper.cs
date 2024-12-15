using System.Data;
using Call.Core.Configuration;

namespace Call.GUI.Common;

public class TableWrapper
{
    private readonly Dictionary<TableType, DataGridView?> _tables = new();

    public void Add(TableType tableType, DataGridView? dataGridView)
    {
        _tables.Add(tableType, dataGridView);
    }

    public void Fill(IReadOnlyDictionary<TableType, List<List<string>>> source)
    {
        foreach (var sourceKey in source.Keys)
            if (_tables.TryGetValue(sourceKey, out var dataGridView) && dataGridView is not null)
                Fill(dataGridView, source[sourceKey]);
    }

    private void Fill(DataGridView dataGridView, List<List<string>> source)
    {
        dataGridView.Rows.Clear();
        foreach (var list in source) dataGridView.Rows.Add(list.ToArray());
    }
}