using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct ResizeEventArgs(Size size)
{
    public Size Size { get; } = size;
}
