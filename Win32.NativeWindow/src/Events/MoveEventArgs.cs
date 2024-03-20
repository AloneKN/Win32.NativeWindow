using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct MoveEventArgs(Point pos)
{
    public Point Position { get; } = pos;
}
