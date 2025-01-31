using Agario.Game.Interfaces;
using SFML.Graphics;

namespace Agario.Infrastructure;

public class Output : IInitializeable
{
    private RenderWindow _renderWindow;
    private Camera _camera;

    public Output(RenderWindow renderWindow)
    {
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        _renderWindow = renderWindow;
        _camera = GameCycle.GetInstance().WorldCamera;
    }

    public void Initialize()
    {
        _renderWindow.SetView(_camera.View);
    }

    public void Display()
    {
        _renderWindow.Clear(new Color(20, 20, 20));
        
        _camera.Update();
        
        _renderWindow.SetView(_camera.View);
        
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