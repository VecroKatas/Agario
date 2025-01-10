using Agario.Infrastructure;
using Agario.Infrastructure.Factories;

namespace Agario.Game;

public class AgarioGame
{
    //new foods arent eatable
    public const int MAX_FOOD_AMOUNT = 200;
    public const int MAX_PLAYERS_AMOUNT = 10;
    
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
}