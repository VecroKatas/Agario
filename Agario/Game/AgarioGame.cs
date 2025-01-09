using Agario.Infrastructure;
using Agario.Infrastructure.Factories;

namespace Agario.Game;

public class AgarioGame
{
    //new foods arent eatable
    public const int MAX_FOOD_AMOUNT = 200;
    
    private PlayingMap _playingMap;

    public Player MainPlayer { get; private set; }
    public List<Player> Players { get; private set; }

    private Random _random = new Random();

    public AgarioGame(PlayingMap playingMap)
    {
        _playingMap = playingMap;
    }

    public void Initialize()
    {
        Players = new List<Player>();
        
        GenerateFood();
        GeneratePlayers();
    }
    
    public void GeneratePlayers()
    {
        MainPlayer = _playingMap.CreatePlayer(true);
        Players.Add(MainPlayer);
    }

    private void GenerateFood()
    {
        while (FoodFactory.FoodsCreated.Count < MAX_FOOD_AMOUNT)
        {
            _playingMap.CreateFood(_random.Next(1, Enum.GetNames(typeof(FoodColor)).Length));
        }
    }

    public void Update()
    {
        //GeneratePlayers();
        GenerateFood();
    }
}