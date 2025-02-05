using Agario.Game.Components;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using Agario.Infrastructure.Utilities;
using SFML.System;

namespace Agario.Game;

public class BotController : Controller, IUpdatable
{
    public PlayerGameObject PlayerGameObject;

    public BotController()
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
    }

    public override void SetTargetGameObject(GameObject gameObject)
    {
        if (TargetGameObject != null && TargetGameObject.GetComponent<Food>().OnBeingEaten != null)
            TargetGameObject.GetComponent<Food>().OnBeingEaten -= DestroyTargetGameObject;
        base.SetTargetGameObject(gameObject);
        PlayerGameObject = TargetGameObject.GetComponent<PlayerGameObject>();
        PlayerGameObject.SizeIncreased = () => {};
        TargetGameObject.GetComponent<Food>().OnBeingEaten += DestroyTargetGameObject;
    }

    public override void DestroyTargetGameObject()
    {
        PlayerGameObject.AgarioGame.PlayerDied(this);
        base.DestroyTargetGameObject();
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
        
        Vector2f closestFoodDirection = TargetGameObject.Shape.Position.CalculateNormalisedDirection(info.ClosestFood.Shape.Position);
        Vector2f ClosestPlayerControllerDirection = TargetGameObject.Shape.Position.CalculateNormalisedDirection(info.ClosestPlayerController.TargetGameObject.Shape.Position);
        
        if (info.FoodDistanceSqr < info.PlayerDistanceSqr)
        {
            return closestFoodDirection;
        }
        
        // i dont like how it looks. so many dots
        if (info.ClosestPlayerController.TargetGameObject.GetComponent<Food>().NutritionValue < TargetGameObject.GetComponent<Food>().NutritionValue)
        {
            return ClosestPlayerControllerDirection;
        }
        
        if (info.ClosestPlayerController.TargetGameObject.GetComponent<Food>().NutritionValue >= TargetGameObject.GetComponent<Food>().NutritionValue)
        {
            return (closestFoodDirection - ClosestPlayerControllerDirection).Normalise();
        }

        return new Vector2f(0, 0);
    }
}