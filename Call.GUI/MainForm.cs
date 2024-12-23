using Call.Core.Configuration;
using Call.Core.Interpreter;
using Call.Core.Lexing;
using Call.Core.Syntax;
using Call.Core.Utilities;
using Call.GUI.Common;
using Call.GUI.Controls;
using Call.GUI.Controls.Styled;
using Call.GUI.Style;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = System.Drawing.Color;
using Console = Call.Core.Configuration.Console;
using MouseButtons = System.Windows.Forms.MouseButtons;

namespace Call.GUI;

public partial class MainForm : BorderlessForm
{
    private ThemeManager _themeManager;

    private CodeTextBox CodeEditor;
    private Terminal RunOutputTerminal;
    private Console _console;

    private DataGridView ReservedTable;
    private DataGridView SeparatorsTable;
    private DataGridView NumbersTable;
    private DataGridView IdentifiersTable;
    private DataGridView TokenTable;
    private RichTextBox TokenView;

    private TreePanel<string> TreePanel;
    private GViewer TreeViewer;

    private DataGridView PolizTable;
    private DataGridView VariablesTable;
    private DataGridView ValuesTable;

    private static void FillTable(DataGridView dgv, IEnumerable<string[]> table)
    {
        dgv.Rows.Clear();
        foreach (var row in table)
            dgv.Rows.Add(row);
    }

    public MainForm()
    {
        InitializeComponent();

        _themeManager = new ThemeManager(Theme.Dark);

        SetupMain();
        SetupMenu();
        SetupBars();
        SetupPanels();
        SetupStatusBar();
        SetupColors();

        DoubleBuffered = true;
        SetupDoubleBuffer(Controls);
    }

    private void SetupMain()
    {
        CodeEditor = new CodeTextBox()
        {
            Dock = DockStyle.Fill,
            BackColor = _themeManager.Theme.ColorScheme.Background,
            ForeColor = _themeManager.Theme.ColorScheme.Text,
            Font = _themeManager.Theme.FontScheme.CodeFont
        };
        MainPanel.Controls.Add(CodeEditor);
    }

    private void SetupStatusBar()
    {
        StatusBar.Controls.Add(new PositionForRTB() { Target = CodeEditor.RichTextBox, Dock = DockStyle.Right });
    }

    private void SetupMenu()
    {
        var buttonGroup = CreateUtils.CreateButtonGroup(DockStyle.Right, FlowDirection.LeftToRight);

        var runProgram = CreateUtils.CreateIconicButton(null, "RunGreen.png");

        var stopProgram = CreateUtils.CreateIconicButton(null, "StopDark.png");

        stopProgram.Visible = false;
        stopProgram.BackColor = Color.Red;

        stopProgram.MaximumSize = runProgram.MaximumSize = new Size(24, 0);

        var isRun = false;
        CancellationTokenSource? cancellationToken = null;
        stopProgram.Click += async (_, _) =>
        {
            if (!isRun) return;
            if (cancellationToken is null) return;
            await cancellationToken.CancelAsync();
            return;
        };
        runProgram.Click += async (_, __) =>
        {
            if (isRun) return;

            isRun = true;
            stopProgram.Show();
            _console.Clear();
            Settings settings = Settings.Default;
            var lexer = new Lexer(CodeEditor.Text, settings, _console);
            var tokens = lexer.Tokens;
            FillLexerTables(settings);
            FillTokens(tokens, settings.Tables);

            if (lexer.HasErrors || tokens.Count == 0)
            {
                isRun = false;
                stopProgram.Hide();
                return;
            }

            var parser = new Parser(settings, tokens, _console);
            FillTreePanel(parser.Pr());
            FillParserTables(parser);

            if (parser.HasErrors)
            {
                isRun = false;
                stopProgram.Hide();
                return;
            }

            cancellationToken = new CancellationTokenSource();
            var task = new Task(() => Interpretator.Execute(parser.Actions, parser.Variables, parser.Values,
                _console,
                cancellationToken.Token), cancellationToken.Token);
            task.ContinueWith(t =>
            {
                stopProgram.Hide();
                isRun = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
            task.Start();
        };

        buttonGroup.AddRange(stopProgram, runProgram);
        MenuPanel.Controls.AddRange(buttonGroup);
    }

    #region Настройка панелей

    private void SetupPanels()
    {
        SetupLeftPanel();
        SetupRightPanel();
        SetupBottomPanel();
    }

    private void SetupLeftPanel()
    {
        var tablePanel = new TableLayoutPanel()
        {
            Dock = DockStyle.Fill,
            Name = "TablePanel"
        };

        SetupLexerTables();

        tablePanel.Controls.AddRange(CreateUtils.WrapControlWithName(ReservedTable, "Reserved"),
            CreateUtils.WrapControlWithName(SeparatorsTable, "Separators"),
            CreateUtils.WrapControlWithName(NumbersTable, "Numbers"),
            CreateUtils.WrapControlWithName(IdentifiersTable, "Identifiers"));
        LeftPanel.AddPage(tablePanel);
    }

    private void SetupRightPanel()
    {
        RightPanel.AddPage(GetTokenPanel());
        RightPanel.AddPage(GetTreePanel());
    }

    private void SetupBottomPanel()
    {
        RunOutputTerminal = new Terminal()
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };
        _themeManager.SetupControl(RunOutputTerminal, false);
        _console = RunOutputTerminal.Console;

        BottomPanel.AddPage("Run", CreateUtils.WrapControlWithName(RunOutputTerminal, "Запуск"));
        BottomPanel.AddPage("ParserPanel", GetParserPanel());
    }

    #endregion

    #region Настройка Баров

    private void SetupBars()
    {
        SetupLeftBar();
        SetupRightBar();
    }

    private void SetupLeftBar()
    {
        SetupLeftTopBar();
        SetupLeftBottomBar();
    }

    private void SetupLeftTopBar()
    {
        var handler = CreateHandler(LeftPanel);
        var buttonGroup = CreateUtils.CreateButtonGroup(DockStyle.Top, FlowDirection.TopDown);
        var tableButton = CreateUtils.CreateIconicButton(null, "TableDark.png");
        tableButton.Click += (s, e) => LeftPanel.SwitchPage("TablePanel");

        buttonGroup.ActiveButtonChanging += handler;
        buttonGroup.Add(tableButton);
        LeftBar.Controls.Add(buttonGroup);
    }

    private void SetupLeftBottomBar()
    {
        var handler = CreateHandler(BottomPanel);
        var buttonGroup = CreateUtils.CreateButtonGroup(DockStyle.Bottom, FlowDirection.BottomUp);

        var runButton = CreateUtils.CreateIconicButton(null, "RunDark.png");
        runButton.Click += (s, e) => BottomPanel.SwitchPage("Run");

        var parserInfoButton = CreateUtils.CreateIconicButton(null, "TableDark.png");
        parserInfoButton.Click += (s, e) => BottomPanel.SwitchPage("ParserPanel");

        buttonGroup.Add(runButton);
        buttonGroup.Add(parserInfoButton);
        buttonGroup.ActiveButtonChanging += handler;
        LeftBar.Controls.Add(buttonGroup);
    }

    private void SetupRightBar()
    {
        SetupRightTopBar();
        // SetupRightBottomBar();
    }

    private void SetupRightTopBar()
    {
        var handler = CreateHandler(RightPanel);
        var buttonGroup = CreateUtils.CreateButtonGroup(DockStyle.Top, FlowDirection.TopDown);

        var tokensButton = CreateUtils.CreateIconicButton(null, "TokenDark.png");
        tokensButton.Click += (s, e) => RightPanel.SwitchPage("Tokens");

        // var descendingTreeButton = CreateUtils.CreateIconicButton(null, "TreeDark.png");
        // descendingTreeButton.Click += (s, e) => RightPanel.SwitchPage("DescendingTree");

        var treeButton = CreateUtils.CreateIconicButton(null, "TreeDark.png");
        treeButton.Click += (s, e) => RightPanel.SwitchPage("TreePanel");

        buttonGroup.AddRange(treeButton, tokensButton);
        buttonGroup.ActiveButtonChanging += handler;
        RightBar.Controls.Add(buttonGroup);
    }

    #endregion

    private void SetupColors()
    {
        BackColor = _themeManager.Theme.ColorScheme.AltBackground;
        ForeColor = _themeManager.Theme.ColorScheme.Text;
        ModifyUtils.SetupColors(_themeManager.Theme.ColorScheme.Text, _themeManager.Theme.ColorScheme.AltBackground,
            true, LeftPanel, LeftBar,
            RightBar, RightPanel, BottomPanel, StatusBar);

        SetupSeparatorColor(Controls);
    }

    private void SetupSeparatorColor(Control.ControlCollection controls)
    {
        foreach (var control in controls)
        {
            if (control is Separator separator)
            {
                separator.BackColor = _themeManager.Theme.ColorScheme.Divider;
                separator.SendToBack();
            }

            if (control is Panel panel)
                SetupSeparatorColor(panel.Controls);
        }
    }

    #region Настройки для отображения результата полиза и т.п

    private void FillParserTables(Parser parser)
    {
        PolizTable.Rows.Clear();
        PolizTable.Columns.Clear();
        var actions = parser.Actions;
        for (var i = 0; i < actions.Count; i++)
            PolizTable.Columns.Add(null, null);
        if (actions.Count > 0)
        {
            PolizTable.Rows.Add(Enumerable.Range(0, actions.Count).Select(x => x.ToString()).ToArray<object>());
            PolizTable.Rows.Add(actions.ToArray());
        }

        VariablesTable.Rows.Clear();
        var vars = parser.Variables;
        foreach (var (key, value) in vars)
            VariablesTable.Rows.Add(key, value);

        ValuesTable.Rows.Clear();
        var values = parser.Values;
        for (var i = 0; i < values.Count; i++) ValuesTable.Rows.Add(i, values[i]);
    }

    private TableLayoutPanel GetParserPanel()
    {
        var panel = new TableLayoutPanel()
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 3
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        PolizTable = CreateUtils.CreateTable(0);
        PolizTable.Dock = DockStyle.Fill;
        PolizTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;


        VariablesTable = CreateUtils.CreateTable(2);
        VariablesTable.Dock = DockStyle.Fill;
        VariablesTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

        ValuesTable = CreateUtils.CreateTable(2);
        ValuesTable.Dock = DockStyle.Fill;
        ValuesTable.AutoSize = true;
        ValuesTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

        _themeManager.SetupControl(PolizTable, false);
        _themeManager.SetupControl(VariablesTable, false);
        _themeManager.SetupControl(ValuesTable, false);

        panel.Controls.Add(CreateUtils.WrapControlWithName(PolizTable, "ПОЛИЗ"));
        panel.Controls.Add(CreateUtils.WrapControlWithName(VariablesTable, "Переменные"));
        panel.Controls.Add(CreateUtils.WrapControlWithName(ValuesTable, "Значения"));
        return panel;
    }

    #endregion

    #region Настройки для панели с деревом

    private static Graph ConvertToGraph<T>(Node<T> root)
    {
        var graph = new Graph();
        var nodeMap = new Dictionary<Node<T>, string>();
        AddNodeToGraph(root);
        return graph;

        void AddNodeToGraph(Node<T> node)
        {
            if (!nodeMap.TryGetValue(node, out var value))
            {
                value = Guid.NewGuid().ToString();
                nodeMap[node] = value;
                graph.AddNode(nodeMap[node]).LabelText = node.Value?.ToString();
            }

            for (var i = node.Children.Count - 1; i >= 0; i--)
            {
                var child = node.Children[i];
                if (!nodeMap.TryGetValue(child, out var value1))
                {
                    value1 = Guid.NewGuid().ToString();
                    nodeMap[child] = value1;
                    graph.AddNode(nodeMap[child]).LabelText = child.Value?.ToString();
                }

                graph.AddEdge(value, value1);

                AddNodeToGraph(child);
            }
        }
    }

    private void FillTreePanel(Node<string>? root)
    {
        // TreePanel.Root = root;
        if (root is null)
        {
            TreeViewer.Graph = null;
            return;
        }

        var graph = ConvertToGraph(root);
        var textColor = new Microsoft.Msagl.Drawing.Color(_themeManager.Theme.ColorScheme.Text.A,
            _themeManager.Theme.ColorScheme.Text.R, _themeManager.Theme.ColorScheme.Text.G,
            _themeManager.Theme.ColorScheme.Text.B);
        var borderColor = new Microsoft.Msagl.Drawing.Color(_themeManager.Theme.ColorScheme.Border.A,
            _themeManager.Theme.ColorScheme.Border.R, _themeManager.Theme.ColorScheme.Border.G,
            _themeManager.Theme.ColorScheme.Border.B);
        var fillColor = new Microsoft.Msagl.Drawing.Color(_themeManager.Theme.ColorScheme.Background.A,
            _themeManager.Theme.ColorScheme.Background.R, _themeManager.Theme.ColorScheme.Background.G,
            _themeManager.Theme.ColorScheme.Background.B);
        var backColor = new Microsoft.Msagl.Drawing.Color(_themeManager.Theme.ColorScheme.AltBackground.A,
            _themeManager.Theme.ColorScheme.AltBackground.R, _themeManager.Theme.ColorScheme.AltBackground.G,
            _themeManager.Theme.ColorScheme.AltBackground.B);
        foreach (var graphNode in graph.Nodes)
        {
            graphNode.Attr.FillColor = fillColor;
            graphNode.Attr.Shape = Shape.Circle;
            graphNode.Attr.Color = borderColor;
            graphNode.Label.FontColor = textColor;
        }

        foreach (var edge in graph.Edges)
        {
            edge.Attr.Color = borderColor;
            edge.Attr.LineWidth = 2;
        }

        graph.Attr.BackgroundColor = backColor;
        graph.Attr.Color = backColor;
        TreeViewer.Graph = graph;
    }

    private Control GetTreePanel()
    {
        // var nodeSettings = new TreeSettings(40, 50, 50, _themeManager.Theme.ColorScheme.Text,
        //     _themeManager.Theme.ColorScheme.Background, _themeManager.Theme.ColorScheme.Border,
        //     _themeManager.Theme.ColorScheme.Border,
        //     _themeManager.Theme.FontScheme.TreeFont);
        TreeViewer = new GViewer()
        {
            Dock = DockStyle.Fill,
            Name = "TreePanel"
        };
        TreeViewer.OutsideAreaBrush = new SolidBrush(_themeManager.Theme.ColorScheme.AltBackground);
        var lastMousePoint = new Point();
        TreeViewer.MouseDown += (_, e) =>
        {
            if (e.Button != MouseButtons.Right)
                return;
            if (e.Clicks == 2)
                TreeViewer.FitGraphBoundingBox();
            else
                lastMousePoint = e.Location;
        };
        TreeViewer.MouseMove += (_, e) =>
        {
            if (e.Button != MouseButtons.Right) return;
            var dx = e.X - lastMousePoint.X;
            var dy = e.Y - lastMousePoint.Y;

            TreeViewer.Pan(dx, dy);

            lastMousePoint = e.Location;
        };


        // var panel = new Panel()
        // {
        //     Dock = DockStyle.Fill,
        //     Name = "TreePanel"
        // };
        // TreePanel = new TreePanel<string>()
        // {
        //     Dock = DockStyle.Fill,
        //     DrawMode = DrawMode.Descending,
        //     // Root = _root,
        //     Settings = nodeSettings
        // };
        return TreeViewer;
    }

    #endregion

    #region Создание панели для отображения токенов

    private void FillTokens(IReadOnlyList<Token> tokens, IReadOnlyDictionary<TableType, List<string>> tables)
    {
        TokenTable.Rows.Clear();
        TokenView.Clear();
        var lastPos = new Position(0, 1, 0, 0);
        foreach (var token in tokens)
            if (tables.TryGetValue(token.Table, out var table))
            {
                var lexem = table[token.Id];
                TokenTable.Rows.Add(token.Table, token.Id + 1, lexem.Replace("\n", "\\n"));

                var curPos = token.Position;
                var lastCol = lastPos.Column + lastPos.Length;
                if (lastPos.Row < curPos.Row)
                    lastCol = 1;

                TokenView.Text += new string(' ', curPos.Column - lastCol) + lexem;
                lastPos = curPos;
            }
    }

    private TableLayoutPanel GetTokenPanel()
    {
        var tokenPanel = new TableLayoutPanel()
        {
            Dock = DockStyle.Fill,
            Name = "Tokens",
            ColumnCount = 1,
            RowCount = 2
        };
        tokenPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        tokenPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
        tokenPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

        TokenTable = CreateUtils.CreateTable(3);
        TokenTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        TokenTable.AllowUserToResizeColumns = false;
        _themeManager.SetupControl(TokenTable, false);

        TokenView = new RichTextBox()
        {
            ReadOnly = true,
            WordWrap = true,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };
        _themeManager.SetupControl(TokenView, false);

        tokenPanel.Controls.Add(TokenTable);
        tokenPanel.Controls.Add(TokenView);
        return tokenPanel;
    }

    #endregion

    #region Заполнение таблиц для лексем

    private void FillLexerTables(Settings settings)
    {
        FillLexerTables(settings.Tables);
    }

    private void FillLexerTables(IReadOnlyDictionary<TableType, List<string>> tables)
    {
        FillTable(ReservedTable,
            tables[TableType.Reserved].Select((str, i) => (string[]) [(i + 1).ToString(), str]));
        FillTable(SeparatorsTable,
            tables[TableType.Separators]
                .Select((str, i) => (string[]) [(i + 1).ToString(), str.Replace("\n", "\\n")]));
        FillTable(NumbersTable,
            tables[TableType.Numbers].Select((str, i) => (string[]) [(i + 1).ToString(), str]));
        FillTable(IdentifiersTable,
            tables[TableType.Identifiers].Select((str, i) => (string[]) [(i + 1).ToString(), str]));
    }

    private void SetupLexerTables()
    {
        ReservedTable = CreateUtils.CreateTable(2);
        _themeManager.SetupControl(ReservedTable, false);
        ReservedTable.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        ReservedTable.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        SeparatorsTable = CreateUtils.CreateTable(2);
        _themeManager.SetupControl(SeparatorsTable, false);
        SeparatorsTable.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        SeparatorsTable.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        NumbersTable = CreateUtils.CreateTable(2);
        _themeManager.SetupControl(NumbersTable, false);
        NumbersTable.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        NumbersTable.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        IdentifiersTable = CreateUtils.CreateTable(2);
        _themeManager.SetupControl(IdentifiersTable, false);
        IdentifiersTable.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        IdentifiersTable.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        FillLexerTables(Settings.Default);
    }

    #endregion

    private static ButtonGroup.ActiveButtonChangingHandler CreateHandler(Control control)
    {
        return (button) =>
        {
            if (control.Visible)
                control.Hide();
            else
                control.Show();
        };
    }
}