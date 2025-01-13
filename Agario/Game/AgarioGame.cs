using Agario.Infrastructure;
using Agario.Game.Factories;
using Agario.Game.Interfaces;
using SFML.System;

namespace Agario.Game;

public class AgarioGame : IGameRules
{
    private const int MAX_FOOD_AMOUNT = 200;
    private const int MAX_PLAYERS_AMOUNT = 10;
    
    private const float SecondsAfterGameOver = 4f;
    
    private Random _random = new Random();
    
    public PlayingMap PlayingMap { get; private set; }
    
    public Player MainPlayer { get; private set; } = null;

    public Action GameOver { get; set; }
    
    private Vector2f _mousePosition;
    private Vector2f _mainPlayerMoveDirection;
    
    public AgarioGame()
    {
        PlayingMap = new PlayingMap();
        
        // Мабуть це виконується в MonoBehaviour
        GameCycle.GetInstance().RegisterObjectToInitialize(this);
        GameCycle.GetInstance().RegisterObjectToPhysicsUpdate(this);
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
    }

    public void Initialize()
    {
        GeneratePlayers();
        GenerateFood();
    }

    public void Start()
    {
        
    }

    public void PhysicsUpdate()
    {
        _mousePosition = GameCycle.GetInstance().InputEvents.MousePosition;
        _mainPlayerMoveDirection = MainPlayer.WorldPosition.CalculatedNormalisedDirection(_mousePosition);
        
        MoveAllPlayers();
    }
    
    public void Update()
    {
        GeneratePlayers();
        GenerateFood();
    }
    
    private void GeneratePlayers()
    {
        if (MainPlayer == null)
            MainPlayer = PlayingMap.CreatePlayer(true);
        
        while (PlayingMap.PlayersOnMap.Count < MAX_PLAYERS_AMOUNT)
        {
            PlayingMap.CreatePlayer(false);
        }
    }

    private void GenerateFood()
    {
        while (PlayingMap.FoodsOnMap.Count < MAX_FOOD_AMOUNT)
        {
            PlayingMap.CreateFood(_random.Next(1, Enum.GetNames(typeof(FoodColor)).Length));
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

    private Vector2f GetBotMoveDirection(Player bot)
    {
        (Food closestFood, float foodDistance) = PlayingMap.GetClosestFoodAndDistance(bot);
        (Player closestPlayer, float playerDistance) = PlayingMap.GetClosestPlayerAndDistance(bot);
        
        Vector2f closestFoodDirection = bot.WorldPosition.CalculatedNormalisedDirection(closestFood.WorldPosition);
        Vector2f closestPlayerDirection = bot.WorldPosition.CalculatedNormalisedDirection(closestPlayer.WorldPosition);
        

        if (foodDistance < playerDistance)
        {
            return closestFoodDirection;
        }
        
        if (closestPlayer.NutritionValue < bot.NutritionValue)
        {
            return closestPlayerDirection;
        }
        
        if (closestPlayer.NutritionValue >= bot.NutritionValue)
        {
            return (closestFoodDirection - closestPlayerDirection).Normalise();
        }

        return new Vector2f(0, 0);
    }
}