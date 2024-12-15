using Call.Core;
using Call.GUI.Common;
using DrawMode = Call.GUI.Common.DrawMode;

namespace Call.GUI;

public partial class TestForm : Form
{
    public TestForm()
    {
        InitializeComponent();
        DoubleBuffered = true;
        var _nodeSettings = new TreeSettings(40, 50, 50, Color.Black, Color.White, Color.Black, Color.Black,
            SystemFonts.DefaultFont);

        var _root = new Node<string>("S");
        _root.Add("T").Add("a");
        _root.Add("+");
        var temp = _root.Add("S");
        temp.Add("T").Add("b");
        temp.Add("+");
        temp.Add("S").Add("T").Add("a");

        var panel = new TreePanel<string>()
        {
            Root = _root,
            Settings = _nodeSettings,
            Dock = DockStyle.Fill,
            DrawMode = DrawMode.Rising
        };
        Controls.Add(panel);
    }
}