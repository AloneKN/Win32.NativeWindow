using System.Drawing;

namespace NativeWindow.Windowing.Events;

public readonly struct ResizeEventArgs(Size size)
{
    public Size Size => size;

    public int Width => Size.Width;

    public int Height => Size.Height;

    public float AspectRatio => size.Width / (float)size.Height;

    public static implicit operator Size(ResizeEventArgs args) => args.Size;
}
