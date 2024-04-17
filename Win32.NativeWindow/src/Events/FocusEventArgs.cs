namespace NativeWindow.Windowing.Events;

public readonly struct FocusEventArgs(bool value)
{
    public bool Focused => value;

    public static implicit operator bool(FocusEventArgs args) => args.Focused;
}
