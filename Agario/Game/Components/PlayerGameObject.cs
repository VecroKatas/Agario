using Agario.Game.Components;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class PlayerGameObject : IComponent
{
    private float _currentMoveSpeed;

    public AgarioGame AgarioGame;

    public GameObject GameObject;
    private Food _food;
    
    public int FoodEaten { get; private set; }
    public int PlayersEaten { get; private set; }

    public Action SizeIncreased { get; set; } = new Action(() =>{});
    private float _lastEatenValue = 0;

    public PlayerGameObject(AgarioGame agarioGame, GameObject gameObject)
    {
        AgarioGame = agarioGame;
        _currentMoveSpeed = GameConfig.PlayerMaxMoveSpeed;
        FoodEaten = 0;
        PlayersEaten = 0;
        GameObject = gameObject;
        
        if (!GameObject.HasComponent<Food>())
        {
            _food = new Food(GameConfig.PlayerMinNutricionalValue);
            GameObject.AddComponent(_food);
        }
    }

    public void SetParentGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public ClosestGameObjectsToPlayerInfo GetClosestGameObjectsInfo()
    {
        return AgarioGame.PlayingMap.GetClosestGameObjectsInfo(GameObject);
    }

    public Vector2f CalculateNextWorldPosition(Vector2f direction)
    {
        return GameObject.Shape.Position + direction * _currentMoveSpeed * Time.DeltaTime;
    }

    public void Move(Vector2f moveDirection)
    {
        moveDirection = AgarioGame.PlayingMap.AdjustMoveDirection(this, moveDirection);
        GameObject.Shape.Position += moveDirection * _currentMoveSpeed * Time.DeltaTime;
    }

    public void EatFood(GameObject other)
    {
        var foodComponent = other.GetComponent<Food>();
        
        IncreaseRadius(foodComponent.NutritionValue);
        ReduceSpeed(foodComponent.NutritionValue);
        
        _food.NutritionValue = GameObject.Shape.Radius;

        if (other.HasComponent<HumanController>())
            PlayersEaten++;
        else
            FoodEaten++;
        
        SizeIncreased.Invoke();
        
        foodComponent.BeingEaten();
    }

    private void IncreaseRadius(float delta)
    {
        if (GameObject.Shape.Radius < GameConfig.PlayerMaxRadius)
        {
            _lastEatenValue = delta * GameConfig.PlayerConsumedFoodValueModifier;
            GameObject.Shape.Radius += delta * GameConfig.PlayerConsumedFoodValueModifier;
            GameObject.Shape.Origin = new Vector2f(GameObject.Shape.Radius, GameObject.Shape.Radius);
        }
    }

    private void ReduceSpeed(float valueConsumed)
    {
        if (_currentMoveSpeed < GameConfig.PlayerMinMoveSpeed)
        {
            float difference = valueConsumed / (GameConfig.PlayerMaxRadius - GameConfig.PlayerMinRadius) * GameConfig.PlayerConsumedFoodValueModifier;
        
            _currentMoveSpeed -= difference * (GameConfig.PlayerMaxMoveSpeed - GameConfig.PlayerMinMoveSpeed);            
        }
    }

    public float GetSizeModifier()
    {
        return 1 + (_lastEatenValue) / (GameConfig.PlayerMaxMoveSpeed - GameConfig.PlayerMinMoveSpeed);
    }
}