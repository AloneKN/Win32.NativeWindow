namespace NativeWindow.Windowing.Events;

public readonly struct KeyboardKeyEventArgs(int scanCode)
{
    public Keys Key { get; } = (Keys)scanCode;

    public int ScanCode { get; } = scanCode;

}
