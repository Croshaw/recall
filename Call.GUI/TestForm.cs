using Call.Core;
using Call.Core.Configuration;
using Call.Core.Lexing;
using Call.Core.Syntax;
using Call.GUI.Common;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = System.Drawing.Color;
using Console = Call.Core.Configuration.Console;
using DrawMode = Call.GUI.Common.DrawMode;

namespace Call.GUI;

public partial class TestForm : Form
{
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

    public TestForm()
    {
        InitializeComponent();
        DoubleBuffered = true;
        const string code = "a+b+a";
        Settings settings = Settings.Default;
        var console = new Console(System.Console.Out, System.Console.Error, System.Console.In);
        var lexer = new Lexer(code, settings, console);
        var parser = new Parser(settings, lexer.Tokens, console);
        var node = parser.Compare().node;

        // var panel = new TreePanel<string>()
        // {
        //     Root = _root,
        //     Settings = _nodeSettings,
        //     Dock = DockStyle.Fill,
        //     DrawMode = DrawMode.Rising
        // };
        // Controls.Add(panel);

        var gview = new GViewer()
        {
            Dock = DockStyle.Fill,
            Graph = ConvertToGraph(node)
        };
        Controls.Add(gview);
    }
}