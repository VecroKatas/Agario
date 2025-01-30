using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using Agario.Infrastructure;
using SFML.System;

namespace Agario.Game;

public class HumanController : PlayerController, IComponent, IUpdatable
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
        //base.SetParentGameObject(gameObject);
        ParentGameObject = gameObject;
        ParentGameObject.RemoveComponent<Controller>();
        ParentGameObject.AddComponent((Controller)this);
        PlayerGameObject = ParentGameObject.GetComponent<PlayerGameObject>();
        PlayerGameObject.SizeIncreased += PlayerGameObjSizeIncreased;
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
        Controller closestBotController = info.ClosestPlayer.GetComponent<Controller>();

        GameObject tmp = closestBotController.ParentGameObject;
        
        closestBotController.SetTargetGameObject(ParentGameObject);
        
        //PlayerGameObject.SizeIncreased = () => { };
        /*ParentGameObject.RemoveComponent<Controller>();
        ParentGameObject.AddComponent((Controller)closestBotController);*/
        SetTargetGameObject(tmp);
        /*ParentGameObject.RemoveComponent<Controller>();
        ParentGameObject.AddComponent((Controller)this);*/
        
        PlayerGameObject.AgarioGame.SetMainPlayer(ParentGameObject);
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