using System.ComponentModel;

namespace Call.GUI.Controls.Styled;

public enum Direction
{
    Vertical,
    Horizontal
}

public class Separator : Panel
{
    private Direction _direction;
    private int _thickness;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Direction Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            Refresh();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Thickness
    {
        get => _thickness;
        set
        {
            _thickness = value;
            Refresh();
        }
    }

    public override void Refresh()
    {
        base.MinimumSize = Size = base.MaximumSize = Direction switch
        {
            Direction.Vertical => new Size(Thickness, 0),
            _ => new Size(0, Thickness)
        };
        base.Refresh();
    }
}