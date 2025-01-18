using Agario.Game;
using Agario.Game.Components;
using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class Output : IInitializeable
{
    private RenderWindow _renderWindow;
    private View _view;
    private GameObject _focusObject;
    private float _reverseZoomModifier = 1;
    private bool _zoomedIn = true;

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

        _focusObject.GetComponent<PlayerComponent>().SizeIncreased += () => ZoomOut();
    }

    public void Display()
    {
        _renderWindow.Clear(new Color(20, 20, 20));

        if (_focusObject != null)
        {
            _view.Center = _focusObject.Shape.Position + _focusObject.Shape.Origin;
            if (_zoomedIn) 
                _zoomedIn = false;
        }
        else
        {
            _view.Center = GameCycle.GetInstance().GetScreenCenter();
            
            if (!_zoomedIn) 
                ZoomOut();
        }
        
        _renderWindow.SetView(_view);
        
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

    private void ZoomOut()
    {
        float zoom = _focusObject.GetComponent<PlayerComponent>().GetSizeModifier();

        _reverseZoomModifier *= zoom;

        _zoomedIn = true;
        
        _view.Zoom(zoom);
    }
}