using Windows.Win32;
using Windows.Win32.Foundation;

namespace NativeWindow.Windowing;

internal readonly struct RegisterParams(FreeLibrarySafeHandle handler, string hashName)
{
    public FreeLibrarySafeHandle Handler => handler;

    public string HashName => hashName;

    public HINSTANCE HInstance => (HINSTANCE)Handler.DangerousGetHandle();
}
