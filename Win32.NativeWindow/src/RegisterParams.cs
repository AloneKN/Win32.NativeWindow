using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using win32 = global::Windows.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

using static Windows.Win32.UI.WindowsAndMessaging.PEEK_MESSAGE_REMOVE_TYPE;
using static Windows.Win32.UI.WindowsAndMessaging.WNDCLASS_STYLES;
using static Windows.Win32.UI.Input.KeyboardAndMouse.VIRTUAL_KEY;

using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_STYLE;
using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_EX_STYLE;
using static Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX;
using static Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD;

namespace NativeWindow.Windowing;

internal readonly struct RegisterParams(FreeLibrarySafeHandle handler, string hashName)
{
    public FreeLibrarySafeHandle Handler { get; } = handler;

    public string HashName { get; } = hashName;

    public HINSTANCE HInstance => (HINSTANCE)Handler.DangerousGetHandle();
}
