using Agario.Game.Interfaces;
using Agario.Infrastructure.Utilities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Agario.Infrastructure;

public struct InputEvents
{
    public Vector2f MousePosition;
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