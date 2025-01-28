using Agario.Game.Components;
using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using Agario.Infrastructure;
using SFML.System;

namespace Agario.Game;

public class BotController : Controller, IUpdatable
{
    public PlayerGameObject PlayerGameObject;

    public BotController()
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
    }

    public override void SetGameObject(GameObject gameObject)
    {
        base.SetGameObject(gameObject);
        PlayerGameObject = GameObject.GetComponent<PlayerGameObject>();
    }

    public void Update()
    {
        if (PlayerGameObject.AgarioGame.PlayingMap.SimulationGoing)
        {
            Move();
        }
    }

    public void Move()
    {
        PlayerGameObject.Move(GetBotMoveDirection());
    }
    
    private Vector2f GetBotMoveDirection()
    {
        ClosestGameObjectsToPlayerInfo info = PlayerGameObject.GetClosestGameObjectsInfo();
        
        Vector2f closestFoodDirection = GameObject.Shape.Position.CalculateNormalisedDirection(info.ClosestFood.Shape.Position);
        Vector2f closestPlayerDirection = GameObject.Shape.Position.CalculateNormalisedDirection(info.ClosestPlayer.Shape.Position);
        
        if (info.FoodDistanceSqr < info.PlayerDistanceSqr)
        {
            return closestFoodDirection;
        }
        
        // i dont like how it looks. so many dots
        if (info.ClosestPlayer.GetComponent<Food>().NutritionValue < GameObject.GetComponent<Food>().NutritionValue)
        {
            return closestPlayerDirection;
        }
        
        if (info.ClosestPlayer.GetComponent<Food>().NutritionValue >= GameObject.GetComponent<Food>().NutritionValue)
        {
            return (closestFoodDirection - closestPlayerDirection).Normalise();
        }

        return new Vector2f(0, 0);
    }
}