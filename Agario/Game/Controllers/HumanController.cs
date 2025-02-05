using Agario.Game.Components;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using Agario.Infrastructure.Utilities;
using SFML.System;

namespace Agario.Game;

public class HumanController : PlayerController, IUpdatable
{
    public PlayerGameObject PlayerGameObject;

    public Action MainPlayerSizeIncreased { get; set; }

    public HumanController(PlayerKeyMap playerKeyMap)
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);

        PlayerInputManager = new PlayerInputManager(playerKeyMap);
        
        PlayerInputManager.AddOnDownKeyBinding(KeyBindAction.PlayerSwap, PlayerSwap);

        MainPlayerSizeIncreased += () => GameCycle.GetInstance().WorldCamera.ZoomOut(PlayerGameObject.GetSizeModifier());
    }

    public override void SetTargetGameObject(GameObject gameObject)
    {
        if (TargetGameObject != null && TargetGameObject.GetComponent<Food>().OnBeingEaten != null)
            TargetGameObject.GetComponent<Food>().OnBeingEaten -= DestroyTargetGameObject;
        base.SetTargetGameObject(gameObject);
        PlayerGameObject = TargetGameObject.GetComponent<PlayerGameObject>();
        PlayerGameObject.SizeIncreased += PlayerGameObjSizeIncreased;
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
            PlayerInputManager.ProcessInput();
            Move();
        }
    }
    
    private void PlayerSwap()
    {
        var info = PlayerGameObject.GetClosestGameObjectsInfo();
        Controller closestBotController = info.ClosestPlayerController;

        GameObject tmp = closestBotController.TargetGameObject;
        
        closestBotController.SetTargetGameObject(TargetGameObject);
        SetTargetGameObject(tmp);
        
        PlayerGameObject.AgarioGame.SetMainPlayer(TargetGameObject);
    }

    public void Move()
    {
        Vector2f mousePosition = GameCycle.GetInstance().InputEvents.MousePosition;
        Vector2f moveDirection = GameCycle.GetInstance().GetScreenCenter().CalculateNormalisedDirection(mousePosition);
        
        PlayerGameObject.Move(moveDirection);
    }

    private void PlayerGameObjSizeIncreased()
    {
        MainPlayerSizeIncreased.Invoke();
    }
}