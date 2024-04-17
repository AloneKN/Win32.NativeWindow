# Win32 native window system for C#

## A native win32 windowing system for Windows. Targeted for use in DirectX applications.


[![Nuget](https://img.shields.io/nuget/v/Win32.NativeWindow)](https://www.nuget.org/packages/Win32.NativeWindow/)

---
#### Usage Examples

###### Namespaces
Using 
```csharp
using NativeWindow.Windowing;
using NativeWindow.Windowing.Events;
```
There are no secrets, it is simple and easy to use, with well-defined event systems.

```csharp
internal class Program(WindowSettings gws) : Window(gws)
{
    protected override void OnUpdateFrame(FrameEventArgs eventArgs)
    {
        base.OnUpdateFrame(eventArgs);
    }

    protected override void OnResize(ResizeEventArgs eventArgs)
    {
        base.OnResize(eventArgs);
    }

    protected override void OnKeyboardKeyDown(KeyboardKeyEventArgs eventArgs)
    {
        base.OnKeyboardKeyDown(eventArgs);

    }
    // protected override void On...

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
```
---

##### License