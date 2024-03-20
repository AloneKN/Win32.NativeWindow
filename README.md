Win32 window system for C#

How to use
```csharp
public class Program : Window
{
    public Program() : base(new GameWindowSettings
    {
        Position = new Point(0),
        Size = new Size(1600, 900),
        Title = "DirectxApp",
        State = WindowState.Normal,
        Border = WindowBorder.Resizable
    })
    {
        //
    }
    protected override void OnUpdateFrame(FrameEventArgs frameEvent)
    {
        base.OnUpdateFrame(frameEvent);

        if(IsKeyPress(Keys.Escape))
        {
            Close();
        }
    }
    protected override void OnRenderFrame(FrameEventArgs FrameTimer)
    {
        base.OnRenderFrame(FrameTimer);
    }
    protected override void OnResize(ResizeEventArgs FrameResize)
    {
        base.OnResize(FrameResize);
    }
    protected override void OnUnload()
    {
        base.OnUnload();
    }

    public static void Main()
    {
        Program program = new Program();
        program.Run();
    }
}
```