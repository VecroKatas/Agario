using System.Reflection.Metadata.Ecma335;
using Agario.Game;
using SFML.Graphics;
using SFML.System;

namespace Agario.Infrastructure;

public struct TextOnDisplay
{
    public Text TextObj;
    public uint FontSize;
    public Color Color;
}

public class Output
{
    private PlayingMap _playingMap;
    private RenderWindow _renderWindow;

    private TextOnDisplay gameOverText;
    private TextOnDisplay statsText;
    private TextOnDisplay timeUntilRestartText;
    
    private Font textFont;
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
        
        textFont = new Font(solutionPath + localFontPath);

        gameOverText = InitText("Game over!", 50, new Color(180, 180, 180), new Vector2f(_renderWindow.Size.X * .41f, _renderWindow.Size.Y * .4f));
        statsText = InitText("Default stats", 30, new Color(160, 160, 160), new Vector2f(_renderWindow.Size.X * .43f, _renderWindow.Size.Y * .5f));
        timeUntilRestartText = InitText("Restart time", 40, new Color(180, 180, 180), new Vector2f(_renderWindow.Size.X * .38f, _renderWindow.Size.Y * .7f));
        
        _renderWindow.Closed += WindowClosed;
    }

    private TextOnDisplay InitText(string content, uint fontSize, Color color, Vector2f position)
    {
        TextOnDisplay textOnDisplay = new TextOnDisplay()
        {
            FontSize = fontSize,
            Color = color
        };
        
        textOnDisplay.TextObj = new Text(content, textFont, textOnDisplay.FontSize);
        textOnDisplay.TextObj.FillColor = textOnDisplay.Color;
        textOnDisplay.TextObj.Origin = new Vector2f(textOnDisplay.FontSize / 2f, textOnDisplay.FontSize / 2f);
        textOnDisplay.TextObj.Position = position;

        return textOnDisplay;
    }

    private TextOnDisplay InitText(string content, TextOnDisplay copyFrom)
    {
        return InitText(content, copyFrom.FontSize, copyFrom.Color, copyFrom.TextObj.Position);
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

    public void DisplayGameOverScreen(float timeUntilRestart, Player mainPlayer)
    {
        _renderWindow.Clear(new Color(20, 20, 20));
        
        UpdateStatsText(mainPlayer);
        UpdateUntilRestartText(timeUntilRestart);
        
        _renderWindow.Draw(gameOverText.TextObj);
        _renderWindow.Draw(statsText.TextObj);
        _renderWindow.Draw(timeUntilRestartText.TextObj);
        
        _renderWindow.Display();
    }

    private void UpdateStatsText(Player mainPlayer)
    {
        string content = "Your size: " + mainPlayer.Shape.Radius + "\n" +
                         "Food eaten: " + mainPlayer.FoodEaten + "\n" +
                         "Players eaten: " + mainPlayer.PlayersEaten;

        statsText = InitText(content, statsText);
    }
    
    private void UpdateUntilRestartText(float timeUntilRestart)
    {
        string content = "Game restarts in: " + timeUntilRestart.ToString("0.00") + "s";

        timeUntilRestartText = InitText(content, timeUntilRestartText);
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