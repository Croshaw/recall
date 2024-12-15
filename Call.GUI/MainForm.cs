﻿using System.Data;
using Call.Core;
using Call.Core.Configuration;
using Call.Core.Utilities;
using Call.GUI.Common;
using Call.GUI.Controls;
using Call.GUI.Controls.Styled;
using Call.GUI.Style;
using Console = Call.Core.Configuration.Console;
using DrawMode = Call.GUI.Common.DrawMode;

namespace Call.GUI;

public partial class MainForm : BorderlessForm
{
    private CodeTextBox CodeEditor;
    private ThemeManager _themeManager;
    private Console _console;
    private TableWrapper _tableWrapper;

    private DataGridView TokenTable;
    private RichTextBox TokenView;

    public MainForm()
    {
        InitializeComponent();

        _themeManager = new ThemeManager(Theme.Dark);
        _tableWrapper = new TableWrapper();

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

        var isRun = false;
        var runProgram = CreateUtils.CreateIconicButton(null, "RunGreen.png");

        var stopProgram = CreateUtils.CreateIconicButton(null, "StopDark.png");

        stopProgram.Visible = false;
        stopProgram.BackColor = Color.Red;

        stopProgram.MaximumSize = runProgram.MaximumSize = new Size(24, 0);

        runProgram.Click += async (_, __) =>
        {
            if (isRun)
                return;
            isRun = true;
            stopProgram.Show();
            _console.Clear();
            var executor = new Executor(_console, CodeEditor.Text);
            var _tables = new Dictionary<TableType, List<List<string>>>();
            foreach (var (tablesKey, value) in executor.Tables)
            {
                var table = value.Select((t, i) => (List<string>) [(i + 1).ToString(), t.Replace("\n", "\\n")])
                    .ToList();
                _tables.Add(tablesKey, table);
            }

            TokenView.Clear();
            TokenTable.Rows.Clear();
            var lastPos = new Position(0, 1, 0, 0);
            foreach (var token in executor.Tokens)
                if (executor.Tables.TryGetValue(token.Table, out var table))
                {
                    var lexem = table[token.Id];
                    TokenTable.Rows.Add(lexem.Replace("\n", "\\n"), token.Table, token.Id + 1);

                    var curPos = token.Position;
                    var lastCol = lastPos.Column + lastPos.Length;
                    if (lastPos.Row < curPos.Row)
                        lastCol = 1;

                    TokenView.Text += new string(' ', curPos.Column - lastCol) + lexem;
                    lastPos = curPos;
                }

            _tableWrapper.Fill(_tables);
            await executor.Execute();
            stopProgram.Hide();
            isRun = false;
        };

        buttonGroup.AddRange(stopProgram, runProgram);
        MenuPanel.Controls.AddRange(buttonGroup);
    }

    private void SetupBars()
    {
        SetupLeftBar();
        SetupRightBar();
    }

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
        ModifyUtils.SetupTablePanel(tablePanel, _tableWrapper, _themeManager);
        var _tables = new Dictionary<TableType, List<List<string>>>();
        var tables = ((Settings)Settings.Default).Tables;

        foreach (var (tablesKey, value) in tables)
        {
            var table = value.Select((t, i) => (List<string>) [(i + 1).ToString(), t.Replace("\n", "\\n")])
                .ToList();
            _tables.Add(tablesKey, table);
        }

        _tableWrapper.Fill(_tables);
        LeftPanel.AddPage(tablePanel);
    }

    private void SetupRightPanel()
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

        TokenTable = CreateUtils.CreateTable(3, _themeManager);
        TokenTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        TokenTable.AllowUserToResizeColumns = false;
        TokenView = new RichTextBox()
        {
            ReadOnly = true,
            WordWrap = true,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };
        tokenPanel.Controls.Add(TokenTable);
        tokenPanel.Controls.Add(TokenView);

        var _nodeSettings = new TreeSettings(40, 50, 50, _themeManager.Theme.ColorScheme.Text,
            _themeManager.Theme.ColorScheme.Background, _themeManager.Theme.ColorScheme.Border,
            _themeManager.Theme.ColorScheme.Border,
            _themeManager.Theme.FontScheme.RunFont);


        var _root = new Node<string>("S");
        _root.Add("T").Add("a");
        _root.Add("+");
        var temp = _root.Add("S");
        temp.Add("T").Add("b");
        temp.Add("+");
        temp.Add("S").Add("T").Add("a");
        var descendingTree = new TreePanel<string>()
        {
            Dock = DockStyle.Fill,
            DrawMode = DrawMode.Descending,
            Root = _root,
            Settings = _nodeSettings,
            Name = "DescendingTree"
        };

        var _root2 = new Node<string>("S");
        var temp2 = _root2.Add("S");
        temp2.Add("T").Add("b");
        temp2.Add("+");
        temp2.Add("S").Add("T").Add("a");
        _root2.Add("+");
        _root2.Add("T").Add("a");
        var risingTree = new TreePanel<string>()
        {
            Dock = DockStyle.Fill,
            DrawMode = DrawMode.Rising,
            Root = _root2,
            Settings = _nodeSettings,
            Name = "RisingTree"
        };

        RightPanel.AddPage(tokenPanel);
        RightPanel.AddPage(descendingTree);
        RightPanel.AddPage(risingTree);
    }

    private void SetupBottomPanel()
    {
        var run = new Terminal()
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };
        _console = run.Console;

        BottomPanel.AddPage("Run", CreateUtils.WrapControlWithName(run, "Run"));
    }

    private ButtonGroup.ActiveButtonChangingHandler CreateHandler(Control control)
    {
        return (button) =>
        {
            if (control.Visible)
                control.Hide();
            else
                control.Show();
        };
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

        buttonGroup.Add(runButton);
        buttonGroup.ActiveButtonChanging += handler;
        LeftBar.Controls.Add(buttonGroup);
    }

    private void SetupRightBar()
    {
        SetupRightTopBar();
        SetupRightBottomBar();
    }

    private void SetupRightTopBar()
    {
        var handler = CreateHandler(RightPanel);
        var buttonGroup = CreateUtils.CreateButtonGroup(DockStyle.Top, FlowDirection.TopDown);

        var tokensButton = CreateUtils.CreateIconicButton(null, "TokenDark.png");
        tokensButton.Click += (s, e) => RightPanel.SwitchPage("Tokens");

        var descendingTreeButton = CreateUtils.CreateIconicButton(null, "TreeDark.png");
        descendingTreeButton.Click += (s, e) => RightPanel.SwitchPage("DescendingTree");

        var risingTreeButton = CreateUtils.CreateIconicButton(null, "TreeDark.png");
        risingTreeButton.Click += (s, e) => RightPanel.SwitchPage("RisingTree");

        buttonGroup.AddRange(descendingTreeButton, risingTreeButton, tokensButton);
        buttonGroup.ActiveButtonChanging += handler;
        RightBar.Controls.Add(buttonGroup);
    }

    private void SetupRightBottomBar()
    {
    }

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
}