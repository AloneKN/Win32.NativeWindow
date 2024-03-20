using System.Drawing;

namespace NativeWindow.Windowing;

public struct GameWindowSettings
{
    public static readonly GameWindowSettings Default = new GameWindowSettings
    {
        Title = "Default",
        Size = new(600, 400),
        Position = new(0, 0),
        State = WindowState.Normal,
        Border = WindowBorder.Resizable
    };

    public string Title { get; set; }
    public Size Size { get; set; }
    public Point Position { get; set; }
    public WindowState State { get; set; }
    public WindowBorder Border { get; set; }
}