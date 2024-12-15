using Call.Core;

namespace Call.GUI.Common;

public struct TreeSettings
{
    public SizeF Size { get; set; }
    public SizeF Offset { get; }
    public Color ForeColor { get; }
    public Color FillColor { get; }
    public Color StrokeColor { get; }
    public Color LineColor { get; }
    public Font Font { get; }


    public TreeSettings(float radius, float offsetX, float offsetY, Color foreColor, Color fillColor, Color strokeColor,
        Color lineColor, Font font) :
        this(radius, radius, offsetX, offsetY, foreColor, fillColor, strokeColor, lineColor, font)
    {
    }

    public TreeSettings(float width, float height, float offsetX, float offsetY, Color foreColor, Color fillColor,
        Color strokeColor, Color lineColor, Font font) :
        this(new SizeF(width, height), new SizeF(offsetX, offsetY), foreColor, fillColor, strokeColor, lineColor, font)
    {
    }

    public TreeSettings(SizeF size, float offsetX, float offsetY, Color foreColor, Color fillColor, Color strokeColor,
        Color lineColor, Font font) :
        this(size, new SizeF(offsetX, offsetY), foreColor, fillColor, strokeColor, lineColor, font)
    {
    }

    public TreeSettings(float radius, SizeF offset, Color foreColor, Color fillColor, Color strokeColor,
        Color lineColor, Font font) :
        this(new SizeF(radius, radius), offset, foreColor, fillColor, strokeColor, lineColor, font)
    {
    }

    public TreeSettings(SizeF size, SizeF offset, Color foreColor, Color fillColor, Color strokeColor, Color lineColor,
        Font font)
    {
        Size = size;
        Offset = offset;
        ForeColor = foreColor;
        FillColor = fillColor;
        StrokeColor = strokeColor;
        LineColor = lineColor;
        Font = font;
    }
}

public static class TreeUtils
{
    public static int Right<T>(Node<T> node)
    {
        if (node.Children.Count == 0)
            return 0;
        if (node.Children.Count == 1)
            return Right<T>(node.Children[0]);
        var n = node.Children.Count;
        if (n % 2 != 0)
            n--;
        n /= 2;
        var count = n;
        for (var i = node.Children.Count - 1; i >= n; i--)
        {
            count += Left(node.Children[i]);
            count += Right(node.Children[i]);
        }

        if (node.Children.Count % 2 != 0)
            count += Right(node.Children[node.Children.Count / 2]);
        return count;
    }

    public static int Left<T>(Node<T> node)
    {
        if (node.Children.Count == 0)
            return 0;
        if (node.Children.Count == 1)
            return Left<T>(node.Children[0]);
        var n = node.Children.Count;
        if (n % 2 != 0)
            n--;
        n /= 2;
        var count = n;
        for (var i = 0; i < n; i++)
        {
            count += Left(node.Children[i]);
            count += Right(node.Children[i]);
        }

        if (node.Children.Count % 2 != 0)
            count += Left(node.Children[node.Children.Count / 2]);
        return count;
    }

    public static List<Node<T>> GetLeaves<T>(Node<T> node)
    {
        if (node.Children.Count == 0)
            return [node];
        var list = new List<Node<T>>();
        foreach (var child in node.Children) list.AddRange(GetLeaves<T>(child));

        return list;
    }

    public static int GetHeight<T>(Node<T>? node)
    {
        if (node is null)
            return 0;
        if (node.Children.Count == 0)
            return 1;
        var max = node.Children.Select(child => GetHeight<T>(child)).Max();
        return max + 1;
    }
}