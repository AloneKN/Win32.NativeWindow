using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct MoveEventArgs(Point pos)
{
    public Point Position => pos;

    public int X => Position.X;

    public int Y => Position.Y;

    public static implicit operator Point(MoveEventArgs args) => args.Position;
}
