using Agario.Game.Components;
using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using Agario.Infrastructure;
using SFML.System;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class PlayerController : IComponent, IUpdatable
{
    public bool IsMainPlayer { get; private set; }

    public PlayerGameObject PlayerGameObject;

    private float _swapCooldown = .5f;
    private float _swapTimer = float.MaxValue;

    public Action MainPlayerSizeIncreased { get; set; }

    public PlayerController(bool isMainPlayer)
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
        
        IsMainPlayer = isMainPlayer;

        MainPlayerSizeIncreased += () => GameCycle.GetInstance().WorldCamera.ZoomOut(PlayerGameObject.GetSizeModifier());
    }

    public void SetGameObject(GameObject gameObject)
    {
        SetGameObject((PlayerGameObject)gameObject);
    }
    
    public void SetGameObject(PlayerGameObject playerGameObject)
    {
        PlayerGameObject = playerGameObject;
        PlayerGameObject.SizeIncreased += PlayerGameObjSizeIncreased;
    }

    public void Update()
    {
        if (PlayerGameObject.PlayingMap.SimulationGoing)
        {
            Move();

            if (_swapTimer > _swapCooldown)
                PlayerSwap();
            else
                UpdateSwapTimer();
        }
    }
    
    private void PlayerSwap()
    {
        bool isFPressed = GameCycle.GetInstance().InputEvents.FPressed;

        if (isFPressed && IsMainPlayer)
        {
            PlayerController closestPlayer = PlayerGameObject.GetClosestGameObjectsInfo().ClosestPlayer.GetComponent<PlayerController>();

            IsMainPlayer = false;
            closestPlayer.IsMainPlayer = true;

            _swapTimer = 0;
            closestPlayer._swapTimer = 0;
        }
    }

    private void UpdateSwapTimer()
    {
        _swapTimer += Time.DeltaTime;
    }

    public void Move()
    {
        Vector2f moveDirection;
        
        if (IsMainPlayer)
        {
            Vector2f mousePosition = GameCycle.GetInstance().InputEvents.MousePosition;
            moveDirection = GameCycle.GetInstance().GetScreenCenter().CalculateNormalisedDirection(mousePosition);
        }
        else
        {
            moveDirection = GetBotMoveDirection();
        }
        
        PlayerGameObject.Move(moveDirection);
    }
    
    private Vector2f GetBotMoveDirection()
    {
        ClosestGameObjectsToPlayerInfo info = PlayerGameObject.GetClosestGameObjectsInfo();
        
        Vector2f closestFoodDirection = PlayerGameObject.Shape.Position.CalculateNormalisedDirection(info.ClosestFood.Shape.Position);
        Vector2f closestPlayerDirection = PlayerGameObject.Shape.Position.CalculateNormalisedDirection(info.ClosestPlayer.Shape.Position);
        
        if (info.FoodDistanceSqr < info.PlayerDistanceSqr)
        {
            return closestFoodDirection;
        }
        
        // i dont like how it looks. so many dots
        if (info.ClosestPlayer.GetComponent<Food>().NutritionValue < PlayerGameObject.GetComponent<Food>().NutritionValue)
        {
            return closestPlayerDirection;
        }
        
        if (info.ClosestPlayer.GetComponent<Food>().NutritionValue >= PlayerGameObject.GetComponent<Food>().NutritionValue)
        {
            return (closestFoodDirection - closestPlayerDirection).Normalise();
        }

        return new Vector2f(0, 0);
    }

    private void PlayerGameObjSizeIncreased()
    {
        if (IsMainPlayer)
            MainPlayerSizeIncreased.Invoke();
    }
}