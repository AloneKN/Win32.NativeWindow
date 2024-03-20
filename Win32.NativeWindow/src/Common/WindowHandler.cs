namespace NativeWindow.Windowing;

public readonly struct WindowHandler(IntPtr value) : IEquatable<WindowHandler>
{
    public readonly IntPtr Value = value;

    public bool IsNull => Value == default;

    public static WindowHandler Null => default;

    public bool Equals(WindowHandler other)
    {
        return this.Value == other.Value;
    }

    public static implicit operator IntPtr(WindowHandler value) => value.Value;

    public static explicit operator WindowHandler(IntPtr value) => new WindowHandler(value);

#pragma warning disable CS8765 // A nulidade do tipo de parâmetro não corresponde ao membro substituído (possivelmente devido a atributos de nulidade).
    public override bool Equals(object obj)
#pragma warning restore CS8765 // A nulidade do tipo de parâmetro não corresponde ao membro substituído (possivelmente devido a atributos de nulidade).
    {
        return obj is WindowHandler other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return this.Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"0x{this.Value:x}";
    }
}
