using Agario.Game.Components;
using Agario.Infrastructure;
using Agario.Game.Factories;
using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using SFML.Graphics;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public struct TextOnDisplay
{
    public Text TextObj;
    public uint FontSize;
    public Color Color;
}

public class AgarioGame : IGameRules
{
    private const int MAX_FOOD_AMOUNT = 200;
    private const int MAX_PLAYERS_AMOUNT = 10;
    
    private const float SecondsAfterGameOver = 4f;
    
    private Random _random = new Random();
    
    public PlayingMap PlayingMap { get; private set; }
    
    public GameObject MainPlayer { get; private set; } = null;

    public Action GameRestart { get; set; }
    
    private Vector2f _mousePosition;
    private Vector2f _mainPlayerMoveDirection;
    private bool _isMainPlayerAlive = true;

    private TextOnDisplay _gameOverText;
    private TextOnDisplay _statsText;
    private TextOnDisplay _timeUntilRestartText;
    
    private Font _textFont;
    private string _solutionPath;
    private const string LocalFontPath = "\\Fonts\\ARIAL.TTF";

    private float _restartTimePassed;

    private readonly GameCycle _gameCycleInstance;
    
    public AgarioGame()
    {
        PlayingMap = new PlayingMap();
        
        _gameCycleInstance = GameCycle.GetInstance();
        
        // Мабуть це виконується в MonoBehaviour
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        GameCycle.GetInstance().RegisterObjectToPhysicsUpdate(this);
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
    }

    public void Initialize()
    {
        PlayingMap.StartSimulation();
        
        GeneratePlayers();
        GenerateFood();

        _restartTimePassed = 0;
        
        _solutionPath = GetSolutionPath();
        
        _textFont = new Font(_solutionPath + LocalFontPath);

        RenderWindow renderWindow = _gameCycleInstance.RenderWindow;
        
        _gameOverText = InitText("Game over!", 50, new Color(180, 180, 180), new Vector2f(renderWindow.Size.X * .41f, renderWindow.Size.Y * .4f));
        _statsText = InitText("Default stats", 30, new Color(160, 160, 160), new Vector2f(renderWindow.Size.X * .43f, renderWindow.Size.Y * .5f));
        _timeUntilRestartText = InitText("Restart time", 40, new Color(180, 180, 180), new Vector2f(renderWindow.Size.X * .38f, renderWindow.Size.Y * .7f));
    }
    
    private void GeneratePlayers()
    {
        MainPlayer ??= PlayingMap.CreatePlayer(true);

        MainPlayer.GetComponent<FoodComponent>().OnBeingEaten += MainPlayerDied;
        
        while (PlayingMap.PlayersOnMap.Count < MAX_PLAYERS_AMOUNT)
        {
            PlayingMap.CreatePlayer(false);
        }
    }

    private void GenerateFood()
    {
        while (PlayingMap.FoodsOnMapCount < MAX_FOOD_AMOUNT)
        {
            PlayingMap.CreateFood(_random.Next(1, Enum.GetNames(typeof(FoodColor)).Length));
        }
    }

    public void PhysicsUpdate()
    {
        if (_isMainPlayerAlive)
        {
            _mousePosition = GameCycle.GetInstance().InputEvents.MousePosition;
            _mainPlayerMoveDirection = MainPlayer.WorldPosition.CalculateNormalisedDirection(_mousePosition);
        
            MoveAllPlayers();
        }
    }
    
    public void Update()
    {
        if (_isMainPlayerAlive)
        {
            GeneratePlayers();
            GenerateFood();
        }
        else
        {
            if (_restartTimePassed < SecondsAfterGameOver)
            {
                UpdateStatsText();
                UpdateUntilRestartText(SecondsAfterGameOver - _restartTimePassed);

                _restartTimePassed += Time.DeltaTime;
            }
            else
            {
                GameRestart.Invoke();
            }
        }
    }
    
    private void MoveAllPlayers()
    {
        foreach (var player in PlayingMap.PlayersOnMap)
        {
            if (player.IsMainPlayer)
            {
                PlayingMap.MovePlayer(player, _mainPlayerMoveDirection);
            }
            else
            {
                PlayingMap.MovePlayer(player, GetBotMoveDirection(player));
            }
        }
    }

    private Vector2f GetBotMoveDirection(PlayerComponent bot)
    {
        ClosestGameObjectsToPlayerInfo info = PlayingMap.GetClosestGameObjectsInfo(bot);
        
        Vector2f closestFoodDirection = bot.GameObject.WorldPosition.CalculateNormalisedDirection(info.ClosestFood.WorldPosition);
        Vector2f closestPlayerDirection = bot.GameObject.WorldPosition.CalculateNormalisedDirection(info.ClosestPlayer.WorldPosition);

        if (info.FoodDistanceSqr < info.PlayerDistanceSqr)
        {
            return closestFoodDirection;
        }
        
        // i dont like how it looks. so many dots
        if (info.ClosestPlayer.GetComponent<FoodComponent>().NutritionValue < bot.GameObject.GetComponent<FoodComponent>().NutritionValue)
        {
            return closestPlayerDirection;
        }
        
        if (info.ClosestPlayer.GetComponent<FoodComponent>().NutritionValue >= bot.GameObject.GetComponent<FoodComponent>().NutritionValue)
        {
            return (closestFoodDirection - closestPlayerDirection).Normalise();
        }

        return new Vector2f(0, 0);
    }

    public List<GameObject> GetGameObjectsToDisplay()
    {
        return _isMainPlayerAlive ? PlayingMap.GameObjectsToDisplay : new List<GameObject>();
    }
    
    public List<Text> GetTextsToDisplay()
    {
        return _isMainPlayerAlive
            ? new List<Text>()
            : new List<Text>()
            {
                _gameOverText.TextObj,
                _statsText.TextObj,
                _timeUntilRestartText.TextObj
            };
    }
    
    private void MainPlayerDied()
    {
        _isMainPlayerAlive = false;

        PlayingMap.StopSimulation();
        PlayingMap.Reset();
    }

    private TextOnDisplay InitText(string content, uint fontSize, Color color, Vector2f position)
    {
        TextOnDisplay textOnDisplay = new TextOnDisplay()
        {
            FontSize = fontSize,
            Color = color
        };
        
        textOnDisplay.TextObj = new Text(content, _textFont, textOnDisplay.FontSize);
        textOnDisplay.TextObj.FillColor = textOnDisplay.Color;
        textOnDisplay.TextObj.Origin = new Vector2f(textOnDisplay.FontSize / 2f, textOnDisplay.FontSize / 2f);
        textOnDisplay.TextObj.Position = position;

        return textOnDisplay;
    }

    private TextOnDisplay InitText(string content, TextOnDisplay copyFrom)
    {
        return InitText(content, copyFrom.FontSize, copyFrom.Color, copyFrom.TextObj.Position);
    }

    private void UpdateStatsText()
    {
        string content = "Your size: " + MainPlayer.Shape.Radius + "\n" +
                         "Food eaten: " + MainPlayer.GetComponent<PlayerComponent>().FoodEaten + "\n" +
                         "Players eaten: " + MainPlayer.GetComponent<PlayerComponent>().PlayersEaten;

        _statsText = InitText(content, _statsText);
    }
    
    private void UpdateUntilRestartText(float timeUntilRestart)
    {
        string content = "Game restarts in: " + timeUntilRestart.ToString("0.00") + "s";

        _timeUntilRestartText = InitText(content, _timeUntilRestartText);
    }
    
    string GetSolutionPath()
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        while (!string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.GetFiles(currentDirectory, "*.sln").Length > 0)
            {
                return currentDirectory;
            }

            currentDirectory = Directory.GetParent(currentDirectory).FullName;
        }

        return "";
    }
}