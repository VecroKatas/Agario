using Agario.Game;
using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class Output : IInitializeable
{
    private RenderWindow _renderWindow;
    private View _view;
    private GameObject _focusObject;

    public Output(RenderWindow renderWindow)
    {
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        _renderWindow = renderWindow;
        _view = new View(new FloatRect(PlayingMap.Width / 2, PlayingMap.Height / 2, 1800, 900));
    }

    public void Initialize()
    {
        _focusObject = GameCycle.GetInstance().GetGameObjectToFocusOn();
        _renderWindow.SetView(_view);
        _view.Center = _focusObject.Shape.Position + _focusObject.Shape.Origin;
    }

    public void Display()
    {
        _renderWindow.Clear(new Color(20, 20, 20));

        if (_focusObject != null)
        {
            _view.Center = _focusObject.Shape.Position + _focusObject.Shape.Origin;
        }
        else
        {
            _view.Center = new Vector2f(PlayingMap.Width / 2, PlayingMap.Height / 2);
        }
        
        List<GameObject> gameObjects = new List<GameObject>(GameCycle.GetInstance().GetGameObjectsToDisplay());
        gameObjects.Reverse();
        
        foreach (var gameObject in gameObjects)
        {
            _renderWindow.Draw(gameObject.Shape);
        }
        
        List<Text> texts = new List<Text>(GameCycle.GetInstance().GetTextsToDisplay());
        
        foreach (var text in texts)
        {
            _renderWindow.Draw(text);
        }
        
        _renderWindow.Display();
    }
}