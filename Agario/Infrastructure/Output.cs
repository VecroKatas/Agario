using SFML.Graphics;

namespace Agario.Infrastructure;

public class Output
{
    private PlayingMap _playingMap;
    private RenderWindow _renderWindow;
    
    private string solutionPath;
    private string localFontPath = "\\Fonts\\ARIAL.TTF";

    public Output(PlayingMap playingMap, RenderWindow renderWindow)
    {
        _playingMap = playingMap;
        _renderWindow = renderWindow;
    }

    public void Initialize()
    {
        solutionPath = GetSolutionPath();
        
        _renderWindow.Closed += WindowClosed;
    }

    public bool IsWindowOpen()
    {
        return _renderWindow.IsOpen;
    }

    public void Display()
    {
        _renderWindow.Clear(new Color(20, 20, 20));

        List<GameObject> objectsToDisplay = new List<GameObject>(_playingMap.GameObjectsToDisplay);
        objectsToDisplay.Reverse();
        
        foreach (var gameObject in objectsToDisplay)
        {
            _renderWindow.Draw(gameObject.Shape);
        }
        
        _renderWindow.Display();
    }
    
    string? GetSolutionPath()
    {
        string? currentDirectory = Directory.GetCurrentDirectory();

        while (!string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.GetFiles(currentDirectory, "*.sln").Length > 0)
            {
                return currentDirectory;
            }

            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        return null;
    }
    
    void WindowClosed(object sender, EventArgs e)
    {
        RenderWindow w = (RenderWindow)sender;
        w.Close();
    }
}