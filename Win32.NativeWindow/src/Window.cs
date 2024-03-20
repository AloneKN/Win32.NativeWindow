using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using win32 = global::Windows.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Graphics.Gdi;

using static Windows.Win32.UI.WindowsAndMessaging.PEEK_MESSAGE_REMOVE_TYPE;
using static Windows.Win32.UI.WindowsAndMessaging.WNDCLASS_STYLES;
using static Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS;
using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_STYLE;
using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_EX_STYLE;
using static Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX;
using static Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD;
using static Windows.Win32.UI.Input.KeyboardAndMouse.VIRTUAL_KEY;
using NativeWindow.Windowing.Events;
using System.Numerics;


namespace NativeWindow.Windowing;

public unsafe class Window
{
    public static WindowProcessEvents.ExternalWindowDelegate? ExternalWindowEvents;
    internal readonly WindowProcessEvents.WindowDelegate WindowDelegateEvent;
    internal HWND handler;
    private RegisterParams registerParams;
    private MSG msg;
    private TimerState timerState;

    public Window(in GameWindowSettings GameWindowSettings)
    {
        timerState = new TimerState();
        WindowDelegateEvent = ProccessEvents;

        #region Register Class Win32

        registerParams = RegisterWin32Class.RegisterClass();

        var tempState = GameWindowSettings.State;
        var tempBorder = tempState is WindowState.FullScreen ? WindowBorder.Hidden : GameWindowSettings.Border;

        handler = PInvoke.CreateWindowEx(
            WS_EX_APPWINDOW,
            registerParams.HashName,
            GameWindowSettings.Title,
            tempState is WindowState.Normal ? GetStyleBorder(tempBorder) : GetStyleBorder(tempBorder) | GetStyle(tempState),
            GameWindowSettings.Position.X,
            GameWindowSettings.Position.Y,
            GameWindowSettings.Size.Width,
            GameWindowSettings.Size.Height,
            default,
            default,
            registerParams.Handler,
            null
        );

        if (handler == IntPtr.Zero)
        {
            throw new Exception($"Failed to create window. Error::{Marshal.GetLastWin32Error()}");
        }

        WindowProcessEvents.Include(this);

        MONITORINFO1.cbSize = (uint)Unsafe.SizeOf<MONITORINFO>();
        PInvoke.GetMonitorInfo(PInvoke.MonitorFromWindow(handler, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY), ref MONITORINFO1);

        Title = GameWindowSettings.Title;
        Size = GameWindowSettings.Size;
        Position = GameWindowSettings.Position;
        _state = tempState;
        _Border = tempBorder;

        PInvoke.UpdateWindow(handler);

        #endregion
    }

    public WindowHandler Handler => new WindowHandler(this.handler.Value);

    private bool _focus;
    public bool Focused
    {
        get => _focus; 
        set
        {
            _focus = value;
            PInvoke.SetWindowPos(handler, HWND.HWND_TOP, 0, 0, 0, 0,
                    SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW | SWP_DRAWFRAME);
        }
    }

    private int _countTitle;
    public string Title
    {
        get
        {
            char[] Text = new char[_countTitle];
            fixed (char* Text_ptr = Text)
            {
                PInvoke.GetWindowText(handler, Text_ptr, _countTitle);
                return new string(Text_ptr);
            }
        }
        set
        {
            _countTitle = value.Length + 1;
            fixed (char* Text = value)
            {
                PInvoke.SetWindowText(handler, Text);
            }
        }
    }
    public Point MousePosition
    {
        get => MouseState.Position;
        set
        {
            MouseState.Position = value;
        }
    }
    public Size Size
    {
        get
        {
            PInvoke.GetClientRect(handler, out var rect);
            return new Size(rect.right, rect.bottom);
        }
        set
        {
            PInvoke.SetWindowPos(handler, default, 0, 0, value.Width, value.Height,
                SWP_NOMOVE |
                SWP_NOZORDER |
                SWP_SHOWWINDOW);
        }
    }
    public Point Position
    {
        get
        {
            PInvoke.GetWindowRect(handler, out var rect);
            return new Point(rect.left, rect.top);
        }
        set
        {
            PInvoke.SetWindowPos(handler, default, value.X, value.Y, 0, 0,
                SWP_NOSIZE |
                SWP_NOZORDER |
                SWP_SHOWWINDOW);
        }
    }
    private Point _wheel = new Point(0, 0);
    public Point MouseWheel
    {
        get
        {
            Point currentWheel = _wheel;

            _wheel = new Point(0, 0);

            int wheelV = 0;
            switch (currentWheel.X)
            {
                case 360:
                    wheelV = 1;
                    break;
                case -360:
                    wheelV = -1;
                    break;
            }

            int wheelH = 0;
            switch (currentWheel.Y)
            {
                case 360:
                    wheelH = 1;
                    break;
                case -360:
                    wheelH = -1;
                    break;
            }

            return new Point(wheelV, wheelH);
        }
        set
        {
            _wheel = value;
        }
    }

    private MONITORINFO MONITORINFO1;

    private WindowState _state;
    public WindowState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (State == value) return;

            _state = value;
            SetWindowState();
        }
    }
    private WindowBorder _Border;
    public WindowBorder Border
    {
        get
        {
            return _Border;
        }
        set
        {
            if (Border == value) return;

            _Border = value;
            SetWindowState();
        }
    }

    public void Close()
    {
        PInvoke.PostQuitMessage(0);
        RegisterWin32Class.Unregister(this.registerParams);
        PInvoke.DestroyWindow(handler);

        WindowProcessEvents.Remove(this);
    }

    private static WINDOW_STYLE GetStyle(WindowState windowState)
    {
        return windowState switch
        {
            WindowState.Maximixed => WS_MAXIMIZE,
            WindowState.Minimized => WS_MINIMIZE,
            WindowState.FullScreen => WS_POPUP,
            _ => throw new Exception()
        };

    }
    private static WINDOW_STYLE GetStyleBorder(WindowBorder windowBorder)
    {
        return windowBorder switch
        {
            WindowBorder.Hidden => WS_OVERLAPPEDWINDOW | WS_SIZEBOX,
            WindowBorder.Resizable => WS_OVERLAPPEDWINDOW,
            WindowBorder.Fixed => WS_BORDER,
            _ => throw new Exception()
        };

    }
    private void SetWindowState()
    {
        switch (State)
        {
            case WindowState.Normal:
                WINDOW_STYLE NorStyle = WS_VISIBLE | GetStyleBorder(Border);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)NorStyle);
                PInvoke.SetWindowPos(handler, default, 0, 0, 0, 0,
                    SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;

            case WindowState.Minimized:

                WINDOW_STYLE MinStyle = WS_VISIBLE | GetStyle(State) | GetStyleBorder(Border);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)MinStyle);
                PInvoke.SetWindowPos(handler, default, 0, 0, 0, 0,
                     SWP_NOZORDER | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;

            case WindowState.Maximixed:

                WINDOW_STYLE MaxStyle = WS_VISIBLE | GetStyle(State) | GetStyleBorder(Border);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)MaxStyle);
                PInvoke.SetWindowPos(handler, HWND.HWND_TOP,
                    MONITORINFO1.rcMonitor.left, MONITORINFO1.rcMonitor.top,
                    MONITORINFO1.rcMonitor.right - MONITORINFO1.rcMonitor.left,
                    MONITORINFO1.rcMonitor.bottom - MONITORINFO1.rcMonitor.top,
                    SWP_NOZORDER | SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;

            case WindowState.FullScreen:
                _Border = WindowBorder.Hidden;
                WINDOW_STYLE FullScreenStyle = WS_VISIBLE | GetStyle(State);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)FullScreenStyle);
                PInvoke.SetWindowPos(handler, HWND.HWND_TOP,
                    MONITORINFO1.rcMonitor.left, MONITORINFO1.rcMonitor.top,
                    MONITORINFO1.rcMonitor.right - MONITORINFO1.rcMonitor.left,
                    MONITORINFO1.rcMonitor.bottom - MONITORINFO1.rcMonitor.top,
                    SWP_NOZORDER | SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;
        }
    }

    public bool IsRuning
    {
        get
        {
            unsafe
            {
                if (PInvoke.PeekMessage(out msg, default, 0, 0, PM_REMOVE) != false)
                {
                    fixed (MSG* ptr_mgs = &msg)
                    {
                        _ = PInvoke.DispatchMessage(ptr_mgs);
                        _ = PInvoke.TranslateMessage(ptr_mgs);
                    }

                    if (msg.message == PInvoke.WM_QUIT)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

    public void Run()
    {
        PInvoke.ShowWindow(handler, SW_NORMAL);
        SetWindowState();

        while (this.IsRuning)
        {
            timerState.Tick();

            var frameEvent = new FrameEventArgs
                (
                    timerState.ElapsedTicks,
                    timerState.ElapsedSeconds,
                    timerState.TotalTicks,
                    timerState.TotalSeconds,
                    timerState.FrameCount,
                    timerState.FramesPerSecond
                );

            this.OnUpdateFrame(frameEvent);
            this.OnRenderFrame(frameEvent);

            //this.ResetEvents();
        }

        this.OnUnload();
    }
    public event Action<FrameEventArgs>? UpdateFrame;

    public event Action<FrameEventArgs>? RenderFrame;

    public event Action? UnLoad;

    public event Action<ResizeEventArgs>? Resize;

    public event Action<MoveEventArgs>? Move;

    public event Action<bool>? Focus;

    public event Action<KeyboardKeyEventArgs>? KeyDown;

    public event Action<KeyboardKeyEventArgs>? KeyUp;

    public event Action<MouseButtonEventArgs>? MouseDown;

    public event Action<MouseButtonEventArgs>? MouseUp;

    public event Action<MouseMoveEventArgs>? MouseMove;

    public event Action<MouseMoveEventArgs>? MouseWheelAction;

    protected virtual void OnUpdateFrame(FrameEventArgs eventArgs)
    {
        this.UpdateFrame?.Invoke(eventArgs);
    }
    protected virtual void OnRenderFrame(FrameEventArgs eventArgs)
    {
        this.RenderFrame?.Invoke(eventArgs);
    }
    protected virtual void OnUnload()
    {
        this.UnLoad?.Invoke();
    }
    protected virtual void OnResize(ResizeEventArgs eventArgs)
    {
        this.Resize?.Invoke(eventArgs);
    }
    protected virtual void OnMove(MoveEventArgs eventArgs)
    {
        this.Move?.Invoke(eventArgs);
    }
    protected virtual void OnFocus(bool isFocus)
    {
        this.Focus?.Invoke(isFocus);
    }
    protected virtual void OnKeyboardKeyDown(KeyboardKeyEventArgs eventArgs)
    {
        this.KeyDown?.Invoke(eventArgs);
    }
    protected virtual void OnKeyboardKeyUp(KeyboardKeyEventArgs eventArgs)
    {
        this.KeyUp?.Invoke(eventArgs);
    }
    protected virtual void OnMouseButtonDown(MouseButtonEventArgs eventArgs)
    {
        this.MouseDown?.Invoke(eventArgs);
    }
    protected virtual void OnMouseButtonUp(MouseButtonEventArgs eventArgs)
    {
        this.MouseUp?.Invoke(eventArgs);
    }

    protected virtual void OnMouseMove(MouseMoveEventArgs eventArgs)
    {
        this.MouseMove?.Invoke(eventArgs);
    }

    protected virtual void OnMouseWheel(MouseMoveEventArgs eventArgs)
    {
        this.MouseWheelAction?.Invoke(eventArgs);
    }

    public KeyboardState KeyboardState { get; private set; } = new KeyboardState();
    public MouseState MouseState { get; private set; } = new MouseState();
    public bool IsKeyDown(Keys Key)
    {
        return KeyboardState.IsKeyDown(Key);
    }
    public bool IsKeyUp(Keys Key)
    {
        return KeyboardState.IsKeyUp(Key);

    }
    public bool IsKeyPress(Keys Key)
    {
        return KeyboardState.IsKeyPress(Key);
    }

    public bool IsMouseButtonDown(MouseButton MouseButton)
    {
        return MouseState.IsButtonDown(MouseButton);
    }
    public bool IsMouseButtonUp(MouseButton MouseButton)
    {
        return MouseState.IsButtonUp(MouseButton);
    }
    public bool IsMouseButtonPress(MouseButton MouseButton)
    {
        return MouseState.IsButtonPress(MouseButton);
    }

    public void ResetEvents()
    {
        this.MouseWheel = default;

    }

    private void ProccessEvents(HWND hWnd, uint message, WPARAM wParam, LPARAM lParam)
    {
        ExternalWindowEvents?.Invoke(&hWnd, message, (IntPtr)wParam.Value, lParam.Value);

        switch (message)
        {
            case PInvoke.WM_CREATE:
                break;

            case PInvoke.WM_DESTROY:
                PInvoke.PostQuitMessage(0);
                break;


            case PInvoke.WM_SIZE:
                {
                    if (wParam == PInvoke.SIZE_MINIMIZED)
                    {
                        this.OnResize(new ResizeEventArgs(Size.Empty));

                    }
                    else
                    {
                        this.OnResize(new ResizeEventArgs(new Size(Win32Helper.LOWORD((nuint)lParam.Value), Win32Helper.HIWORD((nuint)lParam.Value))));
                    }
                }
                break;


            case PInvoke.WM_SETFOCUS:
                {
                    _focus = true;
                    this.OnFocus(true);
                }
                break;

            case PInvoke.WM_KILLFOCUS:
                {
                    _focus = false;
                    this.OnFocus(false);
                }
                break;


            case PInvoke.WM_KEYDOWN:
                {
                    var scanCode = (int)wParam.Value;
                    KeyboardState[scanCode] = true;

                    this.OnKeyboardKeyDown(new KeyboardKeyEventArgs(scanCode));
                }
                break;

            case PInvoke.WM_KEYUP:
                {
                    var scanCode = (int)wParam.Value;
                    KeyboardState[scanCode] = false;


                    this.OnKeyboardKeyUp(new KeyboardKeyEventArgs(scanCode));
                }
                break;

            case PInvoke.WM_SYSKEYDOWN:
                {
                    
                }
                break;
            case PInvoke.WM_SYSKEYUP:
                {
                    
                }
                break;

            case PInvoke.WM_MOUSEMOVE:
                {
                    MouseState.Position = new Point(Win32Helper.GET_X_LPARAM(lParam), Win32Helper.GET_Y_LPARAM(lParam));
                    OnMouseMove(new MouseMoveEventArgs(MouseState.Position));
                }
                break;

            case PInvoke.WM_MOUSEWHEEL:
                {
                    if (Win32Helper.GET_KEYSTATE_WPARAM(wParam) == Win32Helper.MK_SHIFT)
                    {
                        _wheel.X = Win32Helper.GET_WHEEL_DELTA_WPARAM(wParam);
                    }
                    else
                    {
                        _wheel.Y = Win32Helper.GET_WHEEL_DELTA_WPARAM(wParam);
                    }

                    this.OnMouseWheel(new MouseMoveEventArgs(MouseWheel));
                }
                break;

            case PInvoke.WM_LBUTTONDOWN:
                {
                    var scanCode = (int)MouseButton.Button1;
                    MouseState[scanCode] = true;

                    this.OnMouseButtonDown(new MouseButtonEventArgs(scanCode));
                }
                break;
            case PInvoke.WM_LBUTTONUP:
                {
                    var scanCode = (int)MouseButton.Button1;
                    MouseState[scanCode] = false;

                    this.OnMouseButtonUp(new MouseButtonEventArgs(scanCode));
                }
                break;
            case PInvoke.WM_RBUTTONDOWN:
                {
                    var scanCode = (int)MouseButton.Button2;
                    MouseState[scanCode] = true;

                    this.OnMouseButtonDown(new MouseButtonEventArgs(scanCode));
                }
                break;
            case PInvoke.WM_RBUTTONUP:
                {
                    var scanCode = (int)MouseButton.Button2;
                    MouseState[scanCode] = false;

                    this.OnMouseButtonUp(new MouseButtonEventArgs(scanCode));
                }
                break;
            case PInvoke.WM_MBUTTONDOWN:
                {
                    var scanCode = (int)MouseButton.Button3;
                    MouseState[scanCode] = true;

                    this.OnMouseButtonDown(new MouseButtonEventArgs(scanCode));
                }
                break;
            case PInvoke.WM_MBUTTONUP:
                {
                    var scanCode = (int)MouseButton.Button3;
                    MouseState[scanCode] = false;

                    this.OnMouseButtonUp(new MouseButtonEventArgs(scanCode));
                }
                break;

            case PInvoke.WM_XBUTTONDOWN:
                switch (Win32Helper.GET_XBUTTON_WPARAM(wParam))
                {
                    case PInvoke.XBUTTON1:
                        {
                            var scanCode = (int)MouseButton.Button5;
                            MouseState[scanCode] = true;

                            this.OnMouseButtonDown(new MouseButtonEventArgs(scanCode));
                        }
                        break;

                    case PInvoke.XBUTTON2:
                        {
                            var scanCode = (int)MouseButton.Button4;
                            MouseState[scanCode] = true;

                            this.OnMouseButtonDown(new MouseButtonEventArgs(scanCode));
                        }
                        break;
                }
                break;

            case PInvoke.WM_XBUTTONUP:
                switch (Win32Helper.GET_XBUTTON_WPARAM(wParam))
                {
                    case PInvoke.XBUTTON1:
                        {
                            var scanCode = (int)MouseButton.Button5;
                            MouseState[scanCode] = false;

                            this.OnMouseButtonUp(new MouseButtonEventArgs(scanCode));
                        }
                        break;

                    case PInvoke.XBUTTON2:
                        {
                            var scanCode = (int)MouseButton.Button4;
                            MouseState[scanCode] = false;

                            this.OnMouseButtonUp(new MouseButtonEventArgs(scanCode));
                        }
                        break;
                }
                break;
        }
    }
}
