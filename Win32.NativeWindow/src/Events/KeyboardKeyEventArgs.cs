namespace NativeWindow.Windowing.Events;

public readonly struct KeyboardKeyEventArgs(int scanCode, InputAction action, bool altPress)
{
    public Keys Key => (Keys)scanCode;

    public int ScanCode => scanCode;

    public InputAction Action => action;

    public bool AltPressed => altPress;


    public static implicit operator Keys(KeyboardKeyEventArgs e) => e.Key;
}
