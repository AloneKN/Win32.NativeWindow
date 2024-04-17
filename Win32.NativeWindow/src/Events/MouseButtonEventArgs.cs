namespace NativeWindow.Windowing.Events;

public readonly struct MouseButtonEventArgs(int scanCode, InputAction action)
{
    public MouseButton Button => (MouseButton)scanCode;

    public int ScanCode => scanCode;

    public InputAction Action => action;

    public static implicit operator MouseButton(MouseButtonEventArgs e) => e.Button;
}
