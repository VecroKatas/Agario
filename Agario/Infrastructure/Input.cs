using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Agario.Infrastructure;

public struct InputEvents
{
    public Vector2f MousePosition;
}

public struct KeyBind
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

public class Input : IInitializeable
{
    private RenderWindow _renderWindow;

    public Input(RenderWindow renderWindow)
    {
        _renderWindow = renderWindow;
    }
    
    public void Initialize(){}

    public void DispatchEvents()
    {
        _renderWindow.DispatchEvents();
    }

    public InputEvents GetInputEvents()
    {
        InputEvents input = new InputEvents()
        {
            MousePosition = GetMousePosition(),
        };

        return input;
    }

    private Vector2f GetMousePosition()
    {
        return Mouse.GetPosition(_renderWindow).ConvertIntoVector2f();
    }
}