using Windows.Win32.Foundation;

namespace NativeWindow.Windowing;
internal static class Win32Helper
{
    public const int MK_SHIFT = 0x0004;
    public static ushort GET_KEYSTATE_WPARAM(WPARAM wParam) => LOWORD(wParam);
    public static ushort GET_XBUTTON_WPARAM(WPARAM wParam) => HIWORD(wParam);
    public static int GET_X_LPARAM(LPARAM lParam) => (short)LOWORD((nuint)lParam.Value);
    public static int GET_Y_LPARAM(LPARAM lParam) => (short)HIWORD((nuint)lParam.Value);
    public static short GET_WHEEL_DELTA_WPARAM(WPARAM wPARAM) => (short)HIWORD(wPARAM);
    public static ushort HIWORD(nuint l) => (ushort)((l >> 16) & 0xFFFF);
    public static ushort LOWORD(nuint l) => (ushort)(l & 0xFFFF);

    public static bool IsShift(LPARAM lParam) => (lParam & (1L << 16)) != 0;
    public static bool IsControl(LPARAM lParam) => (lParam & (1L << 17)) != 0;
    public static bool IsAlt(LPARAM lParam) => (lParam & (1L << 29)) != 0;
    public static bool IsSuper(LPARAM lParam) => (lParam & (1L << 30)) != 0;
    public static bool IsCapsLock(LPARAM lParam) => (lParam & (1L << 0)) != 0;
    public static bool IsNumLock(LPARAM lParam) => (lParam & (1L << 1)) != 0;
}
