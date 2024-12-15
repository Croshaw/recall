namespace Call.Core;

public class Node<T>
{
    public T Value { get; }
    public Node<T>? Parent { get; }
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
}