using Agario.Game.AnimationSystem;
using Agario.Game.Components;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using Agario.Infrastructure.Systems.Audio;
using Agario.Infrastructure.Utilities;
using SFML.System;

namespace Agario.Game;

public class HumanController : PlayerController, IUpdatable
{
    public PlayerGameObject PlayerGameObject;
    public Action MainPlayerSizeIncreased { get; set; }
    private bool movingSoundStarted = false;

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
        PlayerGameObject.SizeIncreased += (other) => EatOther(other);
        TargetGameObject.GetComponent<Food>().OnBeingEaten += DestroyTargetGameObject;
        
        if (_animator != null)
            _animator.SetParentGameObject(TargetGameObject);
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
        
        AudioPlayer.PlayOnce("Swap", 50f);
    }

    public void Move()
    {
        Vector2f mousePosition = GameCycle.GetInstance().InputEvents.MousePosition;
        Vector2f moveDirection = GameCycle.GetInstance().GetScreenCenter().CalculateNormalisedDirection(mousePosition);
        
        PlayerGameObject.Move(moveDirection);
        
        if (!movingSoundStarted)
        {
            AudioPlayer.PlayLooped("Moving", 10f);
            movingSoundStarted = true;
        }

        if (_animator == null)
        {
            _animator = TargetGameObject.GetComponent<AnimatorBase>();
        }

        _animator.Play("HumanPlayerWalking");
    }

    private void EatOther(GameObject other)
    {
        if (other.HasComponent<PlayerGameObject>())
        {
            AudioPlayer.PlayOnce("PlayerChomp", 50f);
            //_animator.Play("HumanPlayerEating");
        }
        else
            AudioPlayer.PlayOnce("FoodChomp", 80f);
    }
}