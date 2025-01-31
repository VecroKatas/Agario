using SFML.Window;

namespace Agario.Infrastructure;

public class KeyBind
{
    private Keyboard.Key _key;
    private bool _wasPressed;

    private Action _onDown;
    public KeyBind(Keyboard.Key key)
    {
        _key = key;
        _wasPressed = false;
    }

    public bool IsPressed() => Keyboard.IsKeyPressed(_key);

    public bool IsDown()
    {
        if (IsPressed() && !_wasPressed)
        {
            _wasPressed = true;
            return true;
        }

        if (!IsPressed())
            _wasPressed = false;

        return false;
    }

    public void ProcessOnDownCallback() => _onDown.Invoke();

    public void AddOnDownCallback(Action action) => _onDown += action;
    public void ResetOnDownCallback() => _onDown = null;
}