using System.Collections;

namespace NativeWindow.Windowing;

public class KeyboardState
{
    private readonly BitArray _keys = new BitArray((int)Keys.Count);
    private readonly BitArray _keysPrevious = new BitArray((int)Keys.Count);
    public bool this[Keys Keys]
    {
        internal set => _keys[(int)Keys] = value;
        get => _keys[(int)Keys];
    }
    public bool this[int index]
    {
        internal set => _keys[index] = value;
        get => _keys[index];
    }

    public bool IsAnyKeyDown
    {
        get
        {
            for (var i = 0; i < _keys.Length; ++i)
            {
                if (_keys[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
    public bool IsKeyDown(Keys Key)
    {
        return _keys[(int)Key];
    }
    public bool IsKeyUp(Keys Key)
    {
        return !_keys[(int)Key];

    }
    public bool IsKeyPress(Keys Key)
    {
        if (_keysPrevious[(int)Key])
        {
            if (IsKeyDown(Key))
            {
                _keysPrevious[(int)Key] = false;
                return true;
            }
        }
        else if (IsKeyUp(Key))
        {
            _keysPrevious[(int)Key] = true;
        }

        return false;
    }
}
