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
using static NativeWindow.Windowing.WindowProcessEvents;

namespace NativeWindow.Windowing;

internal static unsafe class RegisterWin32Class
{
    public static RegisterParams RegisterClass()
    {
        var handle = PInvoke.GetModuleHandle((string)null!);
        var generatedHash = Guid.NewGuid().ToString().ToUpper();

        var winParams = new RegisterParams(handle, generatedHash);

        fixed (char* lpszClassName = winParams.HashName)
        {
            PCWSTR szCursorName = new((char*)PInvoke.IDC_ARROW);

            var wndClassEx = new WNDCLASSEXW
            {
                cbSize = (uint)Unsafe.SizeOf<WNDCLASSEXW>(),
                style = CS_CLASSDC,
                lpfnWndProc = &ProccessMainWindowEvents,
                hInstance = winParams.HInstance,
                hCursor = PInvoke.LoadCursor((HINSTANCE)IntPtr.Zero, szCursorName),
                hbrBackground = (win32.Graphics.Gdi.HBRUSH)IntPtr.Zero,
                hIcon = (HICON)IntPtr.Zero,
                lpszClassName = lpszClassName

            };

            if (PInvoke.RegisterClassEx(&wndClassEx) is 0)
            {
                throw new InvalidOperationException($"Failed to register window class. Error::{Marshal.GetLastWin32Error()}");
            }
        }

        return winParams;
    }

    public static void Unregister(RegisterParams registerParams)
    {
        PInvoke.UnregisterClass(registerParams.HashName, registerParams.Handler);
    }
}