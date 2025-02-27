using Agario.Game.AnimationSystem;
using Agario.Game.Components;
using Agario.Infrastructure;
using Agario.Infrastructure.Utilities;
using SFML.Graphics;
using SFML.System;

namespace Agario.Game.Factories;

public class PlayerFactory
{
    private Color GetRandomColor()
    {
        Random random = new Random();

        return new Color((byte)random.Next(50, 200), (byte)random.Next(50, 200), (byte)random.Next(50, 200));
    }

    private AgarioGame _agarioGame;
    private PlayingMap _playingMap;

    public PlayerFactory(PlayingMap playingMap, AgarioGame agarioGame)
    {
        _playingMap = playingMap;
        _agarioGame = agarioGame;
    }

    public GameObject CreatePlayer(float defaultRadius, Controller controller)
    {
        Vector2f position = GetValidSpawnCoords();
        
        CircleShape circle = new CircleShape(defaultRadius);
        
        circle.Origin = new Vector2f(defaultRadius, defaultRadius);

        Color newColor;
        
        if (controller.GetType() == typeof(HumanController))
        {
            position = new Vector2f(_playingMap.Width / 2, _playingMap.Height / 2);
            newColor = new Color(200, 200, 200);
        }
        else
        {
            newColor = GetRandomColor();
        }
        
        circle.Position = position;
        circle.FillColor = newColor;

        GameObject newGameObject = new GameObject(circle);
        newGameObject.AddComponent(new PlayerGameObject(_agarioGame, newGameObject));
        
        if (controller.GetType() == typeof(HumanController))
        {
            var animator = newGameObject.AddComponent(new HumanPlayerAnimator() as AnimatorBase);
            animator.Setup(BuildPlayerAnimationGraph());
        }
        else
        {
            var animator = newGameObject.AddComponent(new BotPlayerAnimator() as AnimatorBase);
            animator.Setup(BuildBotAnimationGraph());
        }
        
        controller.SetTargetGameObject(newGameObject);
        
        _playingMap.GameObjectsOnMap.Add(newGameObject);
        _playingMap.ControllersOnMap.Add(controller);

        newGameObject.GetComponent<Food>().OnBeingEaten += () => _playingMap.DeleteGameObject(newGameObject);

        return newGameObject;
    }

    private Vector2f GetValidSpawnCoords()
    {
        Vector2f randomVector = Vector2fUtilities.GetRandomSmallVector();
        
        if (randomVector.X < .01f)
            randomVector.X = .01f;
        if (randomVector.X > .99f)
            randomVector.X = .99f;
        
        if (randomVector.Y < .01f)
            randomVector.Y = .01f;
        if (randomVector.Y > .99f)
            randomVector.Y = .99f;
        
        return new Vector2f( randomVector.X * _playingMap.Width, randomVector.Y * _playingMap.Height);
    }
    
    private AnimationGraph BuildPlayerAnimationGraph()
    {
        //var skin = GetCurrentPlayerSkin();
        var animations = AnimationsManager.Animations;

        return new AnimationGraphBuilder()
            .AddState("AnyState", animations["HumanPlayerIdle"])
            .AddState("HumanPlayerWalkingRight", animations["HumanPlayerWalkingRight"])
            .AddState("HumanPlayerWalkingLeft", animations["HumanPlayerWalkingLeft"])
            .AddState("HumanPlayerWalkingUp", animations["HumanPlayerWalkingUp"])
            .AddState("HumanPlayerWalkingDown", animations["HumanPlayerWalkingDown"])
            .AddState("HumanPlayerEatingRight", animations["HumanPlayerEatingRight"])
            .AddState("HumanPlayerEatingLeft", animations["HumanPlayerEatingLeft"])
            .AddState("HumanPlayerEatingUp", animations["HumanPlayerEatingUp"])
            .AddState("HumanPlayerEatingDown", animations["HumanPlayerEatingDown"])
            .SetInitialState("AnyState")
            .AddTransition("AnyState", "HumanPlayerWalkingRight")
            .AddBoolConditionTo("AnyState", "HumanPlayerWalkingRight", true)
            .AddTransition("AnyState", "HumanPlayerWalkingLeft")
            .AddBoolConditionTo("AnyState", "HumanPlayerWalkingLeft", true)
            .AddTransition("AnyState", "HumanPlayerWalkingUp")
            .AddBoolConditionTo("AnyState", "HumanPlayerWalkingUp", true)
            .AddTransition("AnyState", "HumanPlayerWalkingDown")
            .AddBoolConditionTo("AnyState", "HumanPlayerWalkingDown", true)
            .AddTransition("HumanPlayerWalkingRight", "AnyState")
            .AddBoolConditionTo("HumanPlayerWalkingRight", "AnyState", false)
            .AddTransition("HumanPlayerWalkingLeft", "AnyState")
            .AddBoolConditionTo("HumanPlayerWalkingLeft", "AnyState", false)
            .AddTransition("HumanPlayerWalkingUp", "AnyState")
            .AddBoolConditionTo("HumanPlayerWalkingUp", "AnyState", false)
            .AddTransition("HumanPlayerWalkingDown", "AnyState")
            .AddBoolConditionTo("HumanPlayerWalkingDown", "AnyState", false)
            .AddTransition("AnyState", "HumanPlayerEatingRight")
            .AddBoolConditionTo("AnyState", "HumanPlayerEatingRight", true)
            .AddTransition("AnyState", "HumanPlayerEatingLeft")
            .AddBoolConditionTo("AnyState", "HumanPlayerEatingLeft", true)
            .AddTransition("AnyState", "HumanPlayerEatingUp")
            .AddBoolConditionTo("AnyState", "HumanPlayerEatingUp", true)
            .AddTransition("AnyState", "HumanPlayerEatingDown")
            .AddBoolConditionTo("AnyState", "HumanPlayerEatingDown", true)
            .AddTransition("HumanPlayerEatingRight", "AnyState")
            .AddBoolConditionTo("HumanPlayerEatingRight", "AnyState", false)
            .AddTransition("HumanPlayerEatingLeft", "AnyState")
            .AddBoolConditionTo("HumanPlayerEatingLeft", "AnyState", false)
            .AddTransition("HumanPlayerEatingUp", "AnyState")
            .AddBoolConditionTo("HumanPlayerEatingUp", "AnyState", false)
            .AddTransition("HumanPlayerEatingDown", "AnyState")
            .AddBoolConditionTo("HumanPlayerEatingDown", "AnyState", false)
            .Build();
    }

     private AnimationGraph BuildBotAnimationGraph()
     {
         return BuildPlayerAnimationGraph();
     }
}