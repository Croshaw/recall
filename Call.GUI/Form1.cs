using System.CodeDom.Compiler;
using System.Reflection;
using System.Resources;
using Call.Core.Configuration;
using Call.Core.Interpreter;
using Call.Core.Lexing;
using Call.Core.Syntax;
using Call.GUI.Controls;
using Call.GUI.Style;
using Console = Call.Core.Configuration.Console;
using Executor = Call.Core.Executor;

namespace Call.GUI;

public static class ResourceHelper
{
    public static Image GetEmbeddedImage(string resourceName)
    {
        // Получаем текущую сборку
        var assembly = Assembly.GetExecutingAssembly();

        // Полное имя ресурса должно включать пространство имен
        var fullResourceName = $"{assembly.GetName().Name}.{resourceName}";
        // Открываем поток ресурса
        using (var stream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if (stream == null)
                throw new Exception($"Resource {fullResourceName} not found.");

            // Возвращаем изображение
            return new Bitmap(stream);
        }
    }
}

public partial class Form1 : Form
{
    private Control[] Dividers;


    private readonly Settings _settings;
    private Lexer _lexer;
    private Theme _theme;
    private CodeTextBox CodeTextBox;
    private PositionForRTB PositionRTB;

    public Form1()
    {
        InitializeComponent();
        DoubleBuffered = true;
        _theme = Theme.Dark;
        Dividers =
        [
            BottomPanelSeparator, RightBarSeparator, LeftBarSeparator, RightBarSeparator, RightPanelSeparator,
            LeftPanelSeparator, StatusBarSeparator
        ];

        var closeButton = new ToolStripButton("X")
        {
            Dock = DockStyle.Right,
            DisplayStyle = ToolStripItemDisplayStyle.Text
        };

        var button = new ToolStripButton("Запустить") { };
        Menu.Items.Add(button);
        Menu.Items.Add(closeButton);

        LeftBar.Padding = RightBar.Padding = new Padding(2);
        LeftPanel.Visible = false;
        RightPanel.Visible = false;
        BottomPanel.Visible = false;

        CodeTextBox = new CodeTextBox()
        {
            Dock = DockStyle.Fill,
            WordWrap = true
        };

        PositionRTB = new PositionForRTB()
        {
            Target = CodeTextBox.RichTextBox,
            Dock = DockStyle.Right,
            BorderStyle = BorderStyle.None,
            AutoSize = true,
            Format = "$row$:$col$"
        };

        MainPanel.Controls.Add(CodeTextBox);
        StatusBar.Controls.Add(PositionRTB);

        SetupSeparator(LeftPanelSeparator, LeftPanel);
        SetupSeparator(RightPanelSeparator, RightPanel);
        SetupSeparator(BottomPanelSeparator, BottomPanel);
        SetupColors();
        ButtonsSetup();
    }

    private void SetupSeparator(Panel separator, Panel target)
    {
        var horizontal = separator.Dock == DockStyle.Left || separator.Dock == DockStyle.Right;
        var inversion = separator.Dock == DockStyle.Right || separator.Dock == DockStyle.Bottom ? -1 : 1;
        separator.Cursor = horizontal ? Cursors.SizeWE : Cursors.SizeNS;

        var isMouseDown = false;
        var initialPoint = 0;
        var initialSize = 0;

        separator.MouseDown += (sender, e) =>
        {
            isMouseDown = true;
            initialPoint = horizontal ? Cursor.Position.X : Cursor.Position.Y;
            initialSize = horizontal ? target.Width : target.Height;
        };

        separator.MouseUp += (sender, e) => { isMouseDown = false; };

        separator.MouseMove += (sender, e) =>
        {
            if (isMouseDown)
            {
                var delta = (horizontal ? Cursor.Position.X : Cursor.Position.Y) - initialPoint;
                if (horizontal)
                    target.Width = Math.Max(0, initialSize + delta * inversion);
                else
                    target.Height = Math.Max(0, initialSize + delta * inversion);
            }
        };
    }

    private void SetupColors()
    {
        SetupColors(_theme.Divider, null, Dividers);
        SetupColors(_theme.Background, _theme.Text, MainPanel);
        SetupColors(_theme.AltBackground, _theme.AltText, LeftBar, LeftPanel, RightBar, RightPanel,
            BottomPanel, StatusBar);
        CodeTextBox.SecondaryForeColor = _theme.AltText;
        CodeTextBox.BorderColor = _theme.Border;
    }

    private void SetupColors(Color? backColor = null, Color? foreColor = null, params ReadOnlySpan<Control> controls)
    {
        if (controls.Length == 0 || (!backColor.HasValue && !foreColor.HasValue))
            return;
        foreach (var control in controls)
        {
            control.BackColor = backColor ?? control.BackColor;
            control.ForeColor = foreColor ?? control.ForeColor;
            if (control.Controls.Count == 0)
                continue;
            var childrens = new Control[control.Controls.Count];
            for (var i = 0; i < control.Controls.Count; i++)
                childrens[i] = control.Controls[i];
            SetupColors(backColor, foreColor, childrens);
        }
    }

    private EventHandler CreateButtonClickHandler(params Control[] controls)
    {
        Button? lastButton = null;
        return (sender, args) =>
        {
            if (sender is null || sender is not Button button)
                return;

            if (lastButton == button || lastButton is null)
            {
                if (controls[0].Visible)
                    foreach (var control in controls)
                        control.Hide();
                else
                    foreach (var control in controls)
                        control.Show();
            }

            if (controls[0].Visible)
            {
                if (lastButton is not null)
                {
                    lastButton.BackColor = _theme.AltBackground;
                    lastButton.FlatAppearance.MouseOverBackColor = _theme.Background;
                }

                button.ForeColor = _theme.Text;
                button.BackColor = _theme.Accent;
                button.FlatAppearance.MouseOverBackColor = _theme.Accent;
                lastButton = button;
            }
            else
            {
                button.BackColor = _theme.AltBackground;
                button.FlatAppearance.MouseOverBackColor = _theme.Background;
                lastButton = null;
            }
        };
    }

    private void ButtonsSetup()
    {
        LeftPanelTopButtonsSetup(CreateButtonClickHandler(LeftPanel, LeftPanelSeparator));
        LeftPanelBottomButtonsSetup(CreateButtonClickHandler(BottomPanel, BottomPanelSeparator));
        RightPanelTopButtonsSetup(CreateButtonClickHandler(RightPanel, RightPanelSeparator));
        RightPanelBottomButtonsSetup(null);
    }

    private void LeftPanelTopButtonsSetup(EventHandler handler)
    {
        var tablesButton = new Button()
        {
            Dock = DockStyle.Top,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = _theme.Background },
            BackgroundImage = Image.FromFile("Icons/TableDark.png"),
            BackgroundImageLayout = ImageLayout.Center
        };
        // var testButton = new Button()
        // {
        //     Dock = DockStyle.Top,
        //     FlatStyle = FlatStyle.Flat,
        //     FlatAppearance = { BorderSize = 0, MouseOverBackColor = _theme.Background }
        // };

        tablesButton.Click += handler;
        // testButton.Click += handler;


        // LeftBar.Controls.Add(testButton);
        LeftBar.Controls.Add(tablesButton);
    }

    private void LeftPanelBottomButtonsSetup(EventHandler handler)
    {
        var terminalButton = new Button()
        {
            Dock = DockStyle.Bottom,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = _theme.Background },
            BackgroundImage = Image.FromFile("Icons/TerminalDark.png"),
            BackgroundImageLayout = ImageLayout.Center
        };
        var runButton = new Button()
        {
            Dock = DockStyle.Bottom,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = _theme.Background },
            BackgroundImage = Image.FromFile("Icons/RunDark.png"),
            BackgroundImageLayout = ImageLayout.Center
        };
        terminalButton.Click += handler;
        runButton.Click += handler;

        LeftBar.Controls.Add(runButton);
        LeftBar.Controls.Add(GetSeparatorForBar(DockStyle.Bottom));
        LeftBar.Controls.Add(terminalButton);
    }

    private void RightPanelTopButtonsSetup(EventHandler handler)
    {
        var tokensButton = new Button()
        {
            Dock = DockStyle.Top,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = _theme.Background },
            BackgroundImage = Image.FromFile("Icons/TokenDark.png"),
            BackgroundImageLayout = ImageLayout.Center
        };
        var treeButton = new Button()
        {
            Dock = DockStyle.Top,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = _theme.Background },
            BackgroundImage = Image.FromFile("Icons/TreeDark.png"),
            BackgroundImageLayout = ImageLayout.Center
        };

        tokensButton.Click += handler;
        treeButton.Click += handler;

        RightBar.Controls.Add(treeButton);
        RightBar.Controls.Add(GetSeparatorForBar(DockStyle.Top));
        RightBar.Controls.Add(tokensButton);
    }

    private void RightPanelBottomButtonsSetup(EventHandler handler)
    {
        return;
    }

    private Panel GetSeparatorForBar(DockStyle dock, int size = 6)
    {
        return new Panel() { Dock = dock, Size = new Size(size, size) };
    }
}