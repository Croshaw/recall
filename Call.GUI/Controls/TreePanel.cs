using Call.Core;
using System.ComponentModel;

namespace Call.GUI.Common;

public enum DrawMode
{
    Descending,
    Rising
}

public class TreePanel<T> : Panel
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TreeSettings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            UpdatePositions();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DrawMode DrawMode
    {
        get => _drawMode;
        set
        {
            _drawMode = value;
            UpdatePositions();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Node<T>? Root
    {
        get => _root;
        set
        {
            _root = value;
            TreeHeight = TreeUtils.GetHeight(_root) - 1;
            UpdatePositions();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int TreeHeight { get; private set; }

    // private Dictionary<Node<T>, PointF> _positions = new();
    private Dictionary<Node<T>, Point> _positions = new();
    private Node<T>? _root;
    private TreeSettings _settings;
    private DrawMode _drawMode;

    public TreePanel()
    {
        DoubleBuffered = true;
    }

    private void UpdatePositions()
    {
        _positions.Clear();
        if (Root is not null) CalculatePosition(Root, 0, TreeUtils.Left(Root));
    }

    private void CalculatePosition(Node<T> node, int row, int col)
    {
        if (node.Children.Count == 0 && DrawMode == DrawMode.Rising)
            row = TreeHeight;
        _positions.Add(node, new Point(col, row));

        var hasMiddle = node.Children.Count % 2 != 0;
        var middle = node.Children.Count / 2;

        var offsetCol = 0;

        for (var i = middle - 1; i >= 0; i--)
        {
            var child = node.Children[i];
            offsetCol += TreeUtils.Right(child) + 1;
            CalculatePosition(child, row + 1, col - offsetCol);
            offsetCol += TreeUtils.Left(child);
        }

        offsetCol = 0;
        for (var i = middle + (hasMiddle ? 1 : 0); i < node.Children.Count; i++)
        {
            var child = node.Children[i];
            offsetCol += TreeUtils.Left(child) + 1;
            CalculatePosition(child, row + 1, col + offsetCol);
            offsetCol += TreeUtils.Right(child);
        }

        if (hasMiddle)
            CalculatePosition(node.Children[middle], row + 1, col);
    }

    // private void hh(Node<T> node, float x, float y)
    // {
    //     var point = new PointF(x - Settings.Size.Width / 2,
    //         y - Settings.Size.Height / 2);
    //     _positions.Add(node, point);
    //
    //
    //     var hasMiddle = node.Children.Count % 2 != 0;
    //     var middle = node.Children.Count / 2;
    //
    //     var offsetY = y + Settings.Offset.Height;
    //     var offsetX = 0f;
    //
    //     for (var i = middle - 1; i >= 0; i--)
    //     {
    //         var child = node.Children[i];
    //
    //         offsetX += (TreeUtils.Right(child) + 1) * Settings.Offset.Width;
    //         // _positions.Add(child, new PointF(x - offsetX, offsetY));
    //         hh(child, x - offsetX, offsetY);
    //         offsetX += TreeUtils.Left(child) * Settings.Offset.Width;
    //     }
    //
    //     offsetX = 0;
    //     for (var i = middle + (hasMiddle ? 1 : 0); i < node.Children.Count; i++)
    //     {
    //         var child = node.Children[i];
    //         offsetX += (TreeUtils.Left(child) + 1) * Settings.Offset.Width;
    //         // _positions.Add(child, new PointF(x + offsetX, offsetY));
    //         hh(child, x + offsetX, offsetY);
    //         offsetX += TreeUtils.Right(child) * Settings.Offset.Width;
    //     }
    //
    //     if (hasMiddle)
    //         hh(node.Children[middle], x, offsetY);
    // }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (Root is null)
            return;
        e.Graphics.Clear(BackColor);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        // e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

        using var linePen = new Pen(Settings.LineColor);
        using var fillBrush = new SolidBrush(Settings.FillColor);
        using var strokePen = new Pen(Settings.StrokeColor);
        using var textBrush = new SolidBrush(Settings.ForeColor);

        var rows = _positions.Values.Max(pos => pos.Y);
        var cols = _positions.Values.Max(pos => pos.X);
        var width = cols * Settings.Offset.Width + cols * Settings.Size.Width / 4;
        var height = rows * Settings.Offset.Height + rows * Settings.Size.Height / 2;

        var startX = ClientSize.Width / 2 - width / 2;
        var startY = ClientSize.Height / 2 - height / 2;
        DrawNode(e.Graphics, Root, startX, startY, linePen, fillBrush, strokePen, textBrush);
    }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        Invalidate();
    }

    private void DrawNode(Graphics g, Node<T> node, float offsetX, float offsetY, Pen line, Brush fill, Pen stroke,
        Brush text)
    {
        if (!_positions.TryGetValue(node, out var point))
            return;

        var rectF = new RectangleF(offsetX + point.X * Settings.Offset.Width,
            offsetY + point.Y * Settings.Offset.Height, Settings.Size.Width, Settings.Size.Height);

        foreach (var child in node.Children)
        {
            if (_positions.TryGetValue(child, out var point1))
                g.DrawLine(line, rectF.X + rectF.Width / 2, rectF.Y + rectF.Height / 2,
                    offsetX + point1.X * Settings.Offset.Width + rectF.Width / 2,
                    offsetY + point1.Y * Settings.Offset.Height + rectF.Height / 2);
            DrawNode(g, child, offsetX, offsetY, line, fill, stroke, text);
        }

        g.FillEllipse(fill, rectF);
        g.DrawEllipse(stroke, rectF);
        if (node.Value == null) return;
        var format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;
        g.DrawString(node.Value.ToString(), Settings.Font, text, rectF, format);
    }

    // private void DrawNode<T>(Graphics g, Node<T> node, float x, float y, Pen line, Brush fill, Pen stroke, Brush text)
    // {
    //     var rect = new RectangleF(x - Settings.Size.Width / 2,
    //         y - Settings.Size.Height / 2,
    //         Settings.Size.Width, Settings.Size.Height);
    //
    //     var hasMiddle = node.Children.Count % 2 != 0;
    //     var middle = node.Children.Count / 2;
    //
    //     var offsetY = y + Settings.Offset.Height;
    //     var offsetX = 0f;
    //
    //     for (var i = middle - 1; i >= 0; i--)
    //     {
    //         var child = node.Children[i];
    //
    //         offsetX += (TreeUtils.Right(child) + 1) * Settings.Offset.Width;
    //         g.DrawLine(line, x, y, x - offsetX, offsetY);
    //         DrawNode(g, child, x - offsetX, offsetY, line, fill, stroke, text);
    //         offsetX += TreeUtils.Left(child) * Settings.Offset.Width;
    //     }
    //
    //     offsetX = 0;
    //     for (var i = middle + (hasMiddle ? 1 : 0); i < node.Children.Count; i++)
    //     {
    //         var child = node.Children[i];
    //         offsetX += (TreeUtils.Left(child) + 1) * Settings.Offset.Width;
    //         g.DrawLine(line, x, y, x + offsetX, offsetY);
    //         DrawNode(g, child, x + offsetX, offsetY, line, fill, stroke, text);
    //         offsetX += TreeUtils.Right(child) * Settings.Offset.Width;
    //     }
    //
    //     if (hasMiddle)
    //     {
    //         g.DrawLine(line, x, y, x, offsetY);
    //         DrawNode(g, node.Children[middle], x, offsetY, line, fill, stroke, text);
    //     }
    //
    //     g.FillEllipse(fill, rect);
    //     g.DrawEllipse(stroke, rect);
    //     if (node.Value == null) return;
    //     var format = new StringFormat();
    //     format.Alignment = StringAlignment.Center;
    //     format.LineAlignment = StringAlignment.Center;
    //     g.DrawString(node.Value.ToString(), Settings.Font, text, rect, format);
    // }
}