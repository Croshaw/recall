using System.CodeDom.Compiler;
using Call.Core.Configuration;
using Call.Core.Interpreter;
using Call.Core.Lexing;
using Call.Core.Syntax;
using Console = Call.Core.Configuration.Console;
using Executor = Call.Core.Executor;

namespace Call.GUI;

public partial class Form1 : Form
{
    private readonly Settings _settings;
    private Lexer _lexer;

    public Form1()
    {
        InitializeComponent();
        var button = new ToolStripButton("Запустить");
        menuStrip1.Items.Add(button);
        InitColumns(ReservedTable, 2);
        InitColumns(SepTable, 2);
        InitColumns(NumberTable, 2);
        InitColumns(IdTable, 2);
        InitColumns(TokensTable, 2);
        button.Click += (s, e) =>
        {
            richTextBox1.Clear();
            var console = new Console(new TextBoxStreamWriter(richTextBox1, Color.Black),
                new TextBoxStreamWriter(richTextBox1, Color.Red), new RichTextBoxTextReader(richTextBox1));
            var executor = new Executor(console, CodeRTB.Text);
            console.Dispose();
        };
    }

    private void InitColumns(DataGridView dgv, int columnCount)
    {
        dgv.ColumnHeadersVisible = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.RowHeadersVisible = false;
        dgv.Columns.Clear();
        for (var i = 0; i < columnCount; i++) dgv.Columns.Add(new DataGridViewTextBoxColumn());
    }

    private void FillTable(DataGridView dgv, IReadOnlyList<string> table)
    {
        dgv.Rows.Clear();
        for (var i = 0; i < table.Count; i++) dgv.Rows.Add(i + 1, table[i]);
    }
}