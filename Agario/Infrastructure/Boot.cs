using Agario.Game;
using SFML.Graphics;
using SFML.Window;

namespace Agario.Infrastructure;

public class Boot
{
    private RenderWindow _renderWindow;

    public Boot()
    {
        _renderWindow = new RenderWindow(new VideoMode(PlayingMap.Width, PlayingMap.Height), "AeroHockey");
    }

    public void StartGame()
    {
        GameCycle gameCycleInstance = GameCycle.GetInstance();
        gameCycleInstance.Init(_renderWindow, new AgarioGame());
        gameCycleInstance.StartGameCycle();
    }
}