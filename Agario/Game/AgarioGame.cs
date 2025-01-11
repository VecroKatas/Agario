using Agario.Infrastructure;
using Agario.Infrastructure.Factories;
using SFML.System;

namespace Agario.Game;

public class AgarioGame
{
    private const int MAX_FOOD_AMOUNT = 200;
    private const int MAX_PLAYERS_AMOUNT = 10;
    
    private PlayingMap _playingMap;

    public Player MainPlayer { get; private set; } = null;

    private Random _random = new Random();

    public AgarioGame(PlayingMap playingMap)
    {
        _playingMap = playingMap;
    }

    public void Initialize()
    {
        GeneratePlayers();
        GenerateFood();
    }
    
    public void GeneratePlayers()
    {
        if (MainPlayer == null)
            MainPlayer = _playingMap.CreatePlayer(true);
        
        while (_playingMap.PlayersOnMap.Count < MAX_PLAYERS_AMOUNT)
        {
            _playingMap.CreatePlayer(false);
        }
    }

    private void GenerateFood()
    {
        while (_playingMap.FoodsOnMap.Count < MAX_FOOD_AMOUNT)
        {
            _playingMap.CreateFood(_random.Next(1, Enum.GetNames(typeof(FoodColor)).Length));
        }
    }

    public void Update()
    {
        GeneratePlayers();
        GenerateFood();
    }

    public Vector2f GetBotMoveDirection(Player bot)
    {
        (Food closestFood, float foodDistance) = _playingMap.GetClosestFoodAndDistance(bot);
        (Player closestPlayer, float playerDistance) = _playingMap.GetClosestPlayerAndDistance(bot);
        
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