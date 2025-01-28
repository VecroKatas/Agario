﻿using Agario.Game.Components;
using Agario.Infrastructure;
using Agario.Game.Factories;
using Agario.Game.Interfaces;
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
    private const int MAX_FOOD_AMOUNT = 1000;
    private const int MAX_PLAYERS_AMOUNT = 30;
    
    private const float SecondsAfterGameOver = 4f;
    
    private Random _random = new Random();
    
    public PlayingMap PlayingMap { get; private set; }
    
    /*private PlayerGameObject MainPlayer { 
        get 
        {
            try
            {
                return PlayingMap.PlayersOnMap.First(p => p.IsMainPlayer).PlayerGameObject;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        set
        {
            PlayingMap.PlayersOnMap.First(p => p.IsMainPlayer).PlayerGameObject = value;
        }
    }*/

    private PlayerGameObject _mainPlayer;

    public Action GameRestart { get; set; }
    
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
        PlayingMap = new PlayingMap(this);
        
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
        _mainPlayer ??= CreateMainPLayer();
        
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

    private PlayerGameObject CreateMainPLayer()
    {
        PlayerGameObject mainPlayer = PlayingMap.CreatePlayer(true);
        
        SetMainPlayer(mainPlayer);

        return mainPlayer;
    }

    public void SetMainPlayer(PlayerGameObject player)
    {
        _mainPlayer = player;
    }

    public void PhysicsUpdate()
    {
        
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
                UpdateUntilRestartText(SecondsAfterGameOver - _restartTimePassed);

                _restartTimePassed += Time.DeltaTime;
            }
            else
            {
                GameRestart.Invoke();
            }
        }
        
        UpdateGameObjectToFocusOn();
    }
    
    public void PlayerDied(PlayerGameObject player)
    {
        if (player.GetComponent<PlayerController>().IsMainPlayer)
        {
            _isMainPlayerAlive = false;

            UpdateStatsText(player);

            PlayingMap.StopSimulation();
            PlayingMap.Reset();
        }
    }

    public void UpdateGameObjectToFocusOn()
    {
        _gameCycleInstance.WorldCamera.FocusObject = _isMainPlayerAlive ? _mainPlayer : null;
    }

    public List<GameObject> GetGameObjectsToDisplay()
    {
        return _isMainPlayerAlive ? PlayingMap.GameObjectsOnMap : new List<GameObject>();
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

    private void UpdateStatsText(PlayerGameObject player)
    {
        string content = "Your size: " + player.Shape.Radius + "\n" +
                         "Food eaten: " + player.FoodEaten + "\n" +
                         "Players eaten: " + player.PlayersEaten;

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