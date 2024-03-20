using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct MouseMoveEventArgs(Point pos)
{
    public Point Position { get; } = pos;
}
