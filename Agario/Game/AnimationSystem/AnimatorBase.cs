using Agario.Game.AnimationSystem.Conditions;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.System;

namespace Agario.Game.AnimationSystem;

public class AnimatorBase : IComponent, IUpdatable
{
    public GameObject GameObject;
    
    private int _currentFrame = 0;
    private int _ticksPassed;
    
    private AnimationFSM _stateMachine = new();
    private readonly Dictionary<string, BaseAnimationParameter> _parameters = new();
    
    public AnimatorBase() 
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
    }
    
    public void SetParentGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
        GameObject.Shape.Scale = new Vector2f(2, 2);
    }
    
    public void Setup(AnimationGraph data)
    {
        foreach (var states in data.States)
        {
            _stateMachine.AddState(states);
        }
			
        foreach (var transition in data.Transitions)
        {
            _stateMachine.AddTransition(transition);
        }

        foreach (var parameter in data.Parameters)
        {
            _parameters.TryAdd(parameter.Key, parameter.Value);
        }
        
        _stateMachine.ChangeState(data.InitialState);

        _stateMachine.StateChanged += state => StopPlaying(state.Name);
    }
    
    public void Update()
    {
        _stateMachine.Update();
        var frame = _stateMachine.GetCurrentFrame();
        var texture = _stateMachine.GetCurrentTexture();

        GameObject.Shape.Texture = texture;
        GameObject.Shape.TextureRect = frame;
    }

    public void SetBoolParameter(string name, bool value)
    {
        if (_parameters.TryGetValue(name, out var parameter))
        {
            parameter.SetValue(value);
        }
    }
    
    public virtual void Play(string name)
    {
        SetBoolParameter(name, true);
    }

    public void StopPlaying(string name)
    {
        SetBoolParameter(name, false);
    }
}