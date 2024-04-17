
using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct MouseWheelEventArgs(Point wheel)
{
    public Point Wheel => wheel;

    public int X => wheel.X;

    public int Y => wheel.Y;

    public static implicit operator Point(MouseWheelEventArgs e) => e.Wheel;
}
