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

public static class WindowProcessEvents
{
    public unsafe delegate IntPtr ExternalWindowDelegate(void* windowHandlePtr, uint msg, IntPtr wParam, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate void WindowDelegate(HWND windowHandle, uint message, WPARAM wParam, LPARAM lParam);


    private readonly static Dictionary<HWND, WindowDelegate> ProcEvents = [];

    internal static void Include(Window window)
    {

        ProcEvents.Add(window.handler, window.WindowDelegateEvent);
    }

    internal static void Remove(Window window)
    {
        ProcEvents.Remove(window.handler);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    internal static LRESULT ProccessMainWindowEvents(HWND currentHandleWindow, uint message, WPARAM wParam, LPARAM lParam)
    {
        foreach (var proc in ProcEvents)
        {
            HWND hWnd = proc.Key;

            if (hWnd == currentHandleWindow.Value)
            {
                ProcEvents[proc.Key].Invoke(currentHandleWindow, message, wParam, lParam);
            }
        }

        return PInvoke.DefWindowProc(currentHandleWindow, message, wParam, lParam);
    }
}
