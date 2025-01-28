using Agario.Game.Interfaces;
using Agario.Game.Utilities;
using Agario.Infrastructure;
using SFML.System;
using SFML.Window;
using Time = Agario.Infrastructure.Time;

namespace Agario.Game;

public class HumanController : PlayerController, IComponent, IUpdatable
{
    public PlayerGameObject PlayerGameObject;

    private float _swapCooldown = .5f;
    private float _swapTimer = float.MaxValue;

    public Action MainPlayerSizeIncreased { get; set; }

    public HumanController()
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
        
        PlayerInputManager.AddOnDownKeyBind(new KeyBind(Keyboard.Key.F), PlayerSwap);

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
        SetGameObject(tmp);

        _swapTimer = 0;
            
        PlayerGameObject.AgarioGame.SetMainPlayer(GameObject);
    }

    private void UpdateSwapTimer()
    {
        _swapTimer += Time.DeltaTime;
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