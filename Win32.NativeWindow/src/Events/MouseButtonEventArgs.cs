namespace NativeWindow.Windowing.Events;

public readonly struct MouseButtonEventArgs(int scanCode)
{
    public MouseButton Button { get; } = (MouseButton)scanCode;
    public int ScanCode { get; } = scanCode;

    public override string ToString()
    {
        return string.Format("< Button: {0} ScanCode: {1} >", Button.ToString(), ScanCode);
    }
}
