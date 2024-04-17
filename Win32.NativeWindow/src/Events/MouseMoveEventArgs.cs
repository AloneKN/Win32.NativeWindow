using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct MouseMoveEventArgs(Point pos, Point previousPos)
{
    public Point Position => pos;

    public int X => Position.X;

    public int Y => Position.Y;

    public Point PreviousPosition => previousPos;

    public static implicit operator Point(MouseMoveEventArgs args) => args.Position;
}
