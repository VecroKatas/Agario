using System.Reflection.Metadata.Ecma335;
using Agario.Game;
using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public class Output : IInitializeable
{
    private RenderWindow _renderWindow;

    public Output(RenderWindow renderWindow)
    {
        _renderWindow = renderWindow;
    }

    public void Initialize()
    {
        
    }

    public void Display()
    {
        _renderWindow.Clear(new Color(20, 20, 20));

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