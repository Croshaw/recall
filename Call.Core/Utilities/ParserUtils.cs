using System.Diagnostics;
using System.Text;
using Call.Core.Syntax;

namespace Call.Core.Utilities;

public class ParserUtils
{
    public static List<string> Left<T>(Node<T> node)
    {
        if (node.Children.Count == 0)
            return [];
        List<string> result =
            [node.Value!.ToString()!, string.Join("", node.Children.Select(child => child.Value!.ToString()!))];
        var offset = 0;
        foreach (var child in node.Children)
        {
            var tempList = Left<T>(child);
            if (tempList.Count == 0)
                continue;
            var tmp = 0;
            for (var i = 1; i < tempList.Count; i++)
            {
                var source = result.Last();
                var old = tempList[i - 1];
                var @new = tempList[i];
                result.Add(source.Substring(0, offset) + @new + source.Substring(offset + old.Length));
                tmp = @new.Length + 1;
            }

            offset += tmp;
        }

        return result;
    }

    public static List<string> Right<T>(Node<T> node)
    {
        if (node.Children.Count == 0)
            return [];
        List<string> result =
            [node.Value!.ToString()!, string.Join("", node.Children.Select(child => child.Value!.ToString()!))];
        var offset = result.Last().Length - node.Children.Last().Value!.ToString()!.Length;
        for (var i = node.Children.Count - 1; i >= 0; i--)
        {
            var child = node.Children[i];
            var tempList = Right<T>(child);
            if (tempList.Count == 0)
            {
                offset--;
                continue;
            }

            for (var j = 1; j < tempList.Count; j++)
            {
                var source = result.Last();
                var old = tempList[j - 1];
                var @new = tempList[j];
                result.Add(source.Substring(0, offset) + @new + source.Substring(offset + old.Length));
            }

            offset--;
        }

        return result;
    }

    public static (HashSet<string>, HashSet<string>, Dictionary<string, HashSet<string>>) GetAlph<T>(Node<T> node)
    {
        var name = node.Value!.ToString();
        if (node.Children.Count == 0)
            return ([name!], [], []);
        HashSet<string> terminal = [name!];
        HashSet<string> nonTerminal = [];
        Dictionary<string, HashSet<string>> pravila = new();
        var pravilo = "";
        pravila[name!] = new HashSet<string>();
        foreach (var child in node.Children)
        {
            pravilo += child.Value?.ToString() ?? "";
            var childRes = GetAlph<T>(child);
            foreach (var value in childRes.Item2)
                terminal.Add(value);
            foreach (var value in childRes.Item1)
                nonTerminal.Add(value);
            foreach (var (key, value) in childRes.Item3)
            {
                if (!pravila.ContainsKey(key))
                    pravila[key] = new HashSet<string>();
                var set = pravila[key];
                foreach (var se in value)
                    set.Add(se);
            }
        }

        pravila[name!].Add(pravilo);
        return (nonTerminal, terminal, pravila);
    }

    public static string HzPra(HashSet<string> nonTerminal, HashSet<string> terminal,
        Dictionary<string, HashSet<string>> pravila)
    {
        var sb = new StringBuilder();
        sb.Append("({" + string.Join(", ", nonTerminal) + "}, ");
        sb.Append("{" + string.Join(", ", terminal) + "}, ");
        sb.Append("{" + string.Join("; ", pravila.Select((pair) => $"{pair.Key}->{string.Join("|", pair.Value)}")) +
                  "}, )");
        return sb.ToString();
    }

    public static Node<string> CreateNode(string name)
    {
        return new Node<string>(name);
    }

    public static Node<string> CreateNode()
    {
        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(1);
        return new Node<string>(frame?.GetMethod()?.Name ?? "");
    }
}