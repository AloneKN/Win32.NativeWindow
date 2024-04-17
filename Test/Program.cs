using NativeWindow.Windowing;
using NativeWindow.Windowing.Events;
using System.Drawing;

namespace Test;

internal class Program(WindowSettings gws) : Window(gws)
{
    protected override void OnUpdateFrame(FrameEventArgs eventArgs)
    {
        base.OnUpdateFrame(eventArgs);
    }
    protected override void OnRenderFrame(FrameEventArgs eventArgs)
    {
        base.OnUpdateFrame(eventArgs);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs eventArgs)
    {
        base.OnResize(eventArgs);
    }

    protected override void OnMove(MoveEventArgs eventArgs)
    {
        base.OnMove(eventArgs);
    }

    protected override void OnFocus(FocusEventArgs eventArgs)
    {
        base.OnFocus(eventArgs);
    }

    protected override void OnKeyboardKeyDown(KeyboardKeyEventArgs eventArgs)
    {
        base.OnKeyboardKeyDown(eventArgs);

        Keys key = eventArgs;

        if (key == Keys.Escape)
        {
            base.Close();
        }

        if(eventArgs.AltPressed && key is Keys.Enter)
        {
            if(base.State == WindowState.Normal)
            {
                base.State = WindowState.FullScreen;
            }
            else
            {
                base.State = WindowState.Normal;
            }
        }

    }
    protected override void OnKeyboardKeyUp(KeyboardKeyEventArgs eventArgs)
    {
        base.OnKeyboardKeyUp(eventArgs);
    }

    protected override void OnMouseButtonDown(MouseButtonEventArgs eventArgs)
    {
        base.OnMouseButtonDown(eventArgs);
    }

    protected override void OnMouseButtonUp(MouseButtonEventArgs eventArgs)
    {
        base.OnMouseButtonUp(eventArgs);
    }

    protected override void OnMouseMove(MouseMoveEventArgs eventArgs)
    {
        base.OnMouseMove(eventArgs);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs eventArgs)
    {
        base.OnMouseWheel(eventArgs);
    }

    static void Main(string[] args)
    {
        using var window = new Program(new WindowSettings()
        {
            Border = WindowBorder.Resizable,
            Position = Point.Empty,
            Size = new Size(800, 600),
            State = WindowState.Normal,
            CursorMode = CursorMode.Normal,
            Title = "Test",
            UpdateFrequency = null,
            Icon = WindowResourcePtr.LoadIcon("Content\\win.ico"),
        });

        window.Run();
    }
}


