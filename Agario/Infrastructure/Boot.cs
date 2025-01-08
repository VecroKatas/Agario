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
        GameCycle game = new GameCycle(_renderWindow);
        game.StartGame();
    }
}