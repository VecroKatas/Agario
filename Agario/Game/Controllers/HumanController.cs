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

    public override void SetGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
        PlayerGameObject = GameObject.GetComponent<PlayerGameObject>();
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

        GameObject tmp = closestBotController.GameObject;
        closestBotController.SetGameObject(GameObject);
        PlayerGameObject.SizeIncreased = () => { };
        GameObject.RemoveComponent<Controller>();
        GameObject.AddComponent((Controller)closestBotController);
        SetGameObject(tmp);
        GameObject.RemoveComponent<Controller>();
        GameObject.AddComponent((Controller)this);
            
        PlayerGameObject.AgarioGame.SetMainPlayer(GameObject);
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