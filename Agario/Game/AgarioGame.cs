using Agario.Game.Configs;
using Agario.Infrastructure;
using Agario.Game.Factories;
using Agario.Game.Interfaces;
using Agario.Infrastructure.Systems.Audio;
using Agario.Infrastructure.Utilities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
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
    private PlayerKeyMap playerKeyMap = new PlayerKeyMap()
    {
        KeyBinds = new Dictionary<KeyBindAction, KeyBind>()
        {
            {KeyBindAction.PlayerSwap, new KeyBind(Keyboard.Key.F)}
        }
    };
    
    private Random _random = new Random();
    
    public PlayingMap PlayingMap { get; private set; }

    private GameObject _mainPlayer;
    private HumanController _humanController;

    public Action GameRestart { get; set; }
    
    private bool _isMainPlayerAlive;

    private TextOnDisplay _gameOverText;
    private TextOnDisplay _statsText;
    private TextOnDisplay _timeUntilRestartText;
    
    private Font _textFont;
    private string _localFontPath;

    private float _restartTimePassed;

    private readonly GameCycle _gameCycleInstance;

    private int _maxPlayersOnMap;
    private int _maxFoodsOnMap;
    
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
        _maxPlayersOnMap = PlayingMapConfig.MaxPlayersAmountOnMap;
        _maxFoodsOnMap = PlayingMapConfig.MaxFoodsAmountOnMap;
        _localFontPath = FontsConfig.ArialPath;
        
        PlayingMap.StartSimulation();
        
        InitializeControllers();
        
        GeneratePlayers();
        GenerateFood();

        _restartTimePassed = 0;
        
        _textFont = new Font(AppContext.BaseDirectory + _localFontPath);

        RenderWindow renderWindow = _gameCycleInstance.RenderWindow;
        
        _gameOverText = InitText("Game over!", 50, new Color(180, 180, 180), new Vector2f(renderWindow.Size.X * .41f, renderWindow.Size.Y * .4f));
        _statsText = InitText("Default stats", 30, new Color(160, 160, 160), new Vector2f(renderWindow.Size.X * .43f, renderWindow.Size.Y * .5f));
        _timeUntilRestartText = InitText("Restart time", 40, new Color(180, 180, 180), new Vector2f(renderWindow.Size.X * .38f, renderWindow.Size.Y * .7f));
        
        AudioSystem.PlayOnce(SoundTypes.GameStart);
    }

    private void InitializeControllers()
    {
        _humanController = new HumanController(playerKeyMap);
        PlayingMap.EmptyControllers.Add(_humanController);
        
        while (PlayingMap.EmptyControllers.Count < _maxPlayersOnMap)
        {
            PlayingMap.EmptyControllers.Add(new BotController());
        }
    }
    
    private void GeneratePlayers()
    {
        foreach (var controller in new List<Controller>(PlayingMap.EmptyControllers))
        {
            PlayingMap.CreatePlayer(controller);
            PlayingMap.EmptyControllers.SwapRemove(controller);

            if (controller.GetType() == typeof(HumanController))
            {
                SetMainPlayer(controller.TargetGameObject);
                _isMainPlayerAlive = true;
            }
        }
    }

    private void GenerateFood()
    {
        while (PlayingMap.FoodsOnMapCount < _maxFoodsOnMap)
        {
            PlayingMap.CreateFood(_random.Next(1, Enum.GetNames(typeof(FoodColor)).Length));
        }
    }

    public void SetMainPlayer(GameObject player)
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
            if (_restartTimePassed < GameConfig.SecondsAfterGameOver)
            {
                UpdateUntilRestartText(GameConfig.SecondsAfterGameOver - _restartTimePassed);

                _restartTimePassed += Time.DeltaTime;
            }
            else
            {
                GameRestart.Invoke();
            }
        }
        
        UpdateGameObjectToFocusOn();
    }
    
    public void PlayerDied(Controller controller)
    {
        if (controller.GetType() == typeof(HumanController))
        {
            _isMainPlayerAlive = false;

            UpdateStatsText(controller.TargetGameObject);

            PlayingMap.StopSimulation();
            PlayingMap.Reset();
            
            AudioSystem.StopAllSounds();
            AudioSystem.PlayOnce(SoundTypes.GameOver);
        }
        else
        {
            PlayingMap.ControllersOnMap.SwapRemove(controller);
            PlayingMap.EmptyControllers.Add(controller);
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

    private void UpdateStatsText(GameObject player)
    {
        PlayerGameObject playerGameObject = player.GetComponent<PlayerGameObject>();
        
        string content = "Your size: " + player.Shape.Radius + "\n" +
                         "Food eaten: " + playerGameObject.FoodEaten + "\n" +
                         "Players eaten: " + playerGameObject.PlayersEaten;

        _statsText = InitText(content, _statsText);
    }
    
    private void UpdateUntilRestartText(float timeUntilRestart)
    {
        string content = "Game restarts in: " + timeUntilRestart.ToString("0.00") + "s";

        _timeUntilRestartText = InitText(content, _timeUntilRestartText);
    }
}