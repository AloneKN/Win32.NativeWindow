
using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct MouseWheelEventArgs(Point wheel)
{
    public Point Wheel { get; } = wheel;
}
