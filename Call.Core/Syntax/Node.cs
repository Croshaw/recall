namespace Call.Core.Syntax;

public class Node<T>
{
    public T Value { get; }
    public Node<T>? Parent { get; private set; }
    public List<Node<T>> Children { get; }

    public Node(T value, Node<T>? parent = null)
    {
        Value = value;
        Parent = parent;
        Children = [];
    }

    public Node<T> Add(T value)
    {
        var node = new Node<T>(value, this);
        Children.Add(node);
        return node;
    }

    public Node<T> Add(Node<T> node)
    {
        node.Parent = this;
        Children.Add(node);
        return node;
    }
}