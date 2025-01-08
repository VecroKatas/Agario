using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Agario.Infrastructure;

public class Input
{
    private RenderWindow _renderWindow;

    public Input(RenderWindow renderWindow)
    {
        _renderWindow = renderWindow;
    }

    public void DispatchEvents()
    {
        _renderWindow.DispatchEvents();
    }

    public Vector2f GetMousePosition()
    {
        if (Mouse.IsButtonPressed(Mouse.Button.Left))
            return Mouse.GetPosition(_renderWindow).ConvertIntoVector2f();

        return new Vector2f(-1, -1);
    }
}