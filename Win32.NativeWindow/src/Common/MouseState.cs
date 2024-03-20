using System.Collections;
using System.Drawing;

namespace NativeWindow.Windowing;

public class MouseState
{
    private readonly BitArray _mouse = new BitArray((int)MouseButton.Count);
    private readonly BitArray _mousePrevious = new BitArray((int)MouseButton.Count);

    public Point Position;
    public Point Wheel;

    public bool this[MouseButton MouseButton]
    {
        internal set => _mouse[(int)MouseButton] = value;
        get => _mouse[(int)MouseButton];
    }
    public bool this[int index]
    {
        internal set => _mouse[index] = value;
        get => _mouse[index];
    }
    
    public bool IsAnyButtonDown
    {
        get
        {
            for (var i = 0; i < _mouse.Length; ++i)
            {
                if (_mouse[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
    public bool IsButtonDown(MouseButton MouseButton)
    {
        return _mouse[(int)MouseButton];
    }
    public bool IsButtonUp(MouseButton MouseButton)
    {
        return !_mouse[(int)MouseButton];

    }
    public bool IsButtonPress(MouseButton MouseButton)
    {
        if (_mousePrevious[(int)MouseButton])
        {
            if (IsButtonDown(MouseButton))
            {
                _mousePrevious[(int)MouseButton] = false;
                return true;
            }
        }
        else if (IsButtonUp(MouseButton))
        {
            _mousePrevious[(int)MouseButton] = true;
        }

        return false;
    }
}
