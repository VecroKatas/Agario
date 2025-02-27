using SFML.Graphics;

namespace Agario.Game.AnimationSystem;

public class AnimationFSM
{
    private readonly Dictionary<string, AnimationState> _states = new();
    private readonly Dictionary<AnimationState, List<AnimationTransition>> _transitionMap = new();

    private AnimationState? _currentState;

    public Action<AnimationState> StateChanged;

    public void AddState(AnimationState state)
    {
        _transitionMap.TryAdd(state, new());
        _states.TryAdd(state.Name, state);
    }

    public void AddTransition(AnimationTransition transition)
    {
        if (!_transitionMap.TryGetValue(transition.FromState, out var transitions))
        {
            return;
        }
			
        transitions.Add(transition);
    }

    public void ChangeState(string stateName)
    {
        if (!_states.TryGetValue(stateName, out var newState) || _currentState == newState)
        {
            return;			
        }

        TrySwitchState(newState);
    }

    public void ChangeState(AnimationState newState)
    {
        if (!_states.ContainsValue(newState) || _currentState == newState)
        {
            return;			
        }

        TrySwitchState(newState);
    }

    public void Update()
    {
        if (!_currentState.AnimationNotFinished)
        {
            if (_transitionMap.TryGetValue(_currentState, out var transitions))
            {
                foreach (var transition in transitions)
                {
                    if (transition.CanTransition())
                    {
                        TrySwitchState(transition.ToState);

                        break;
                    }
                }
            }
        
            if (_transitionMap.TryGetValue(_states["AnyState"], out transitions))
            {
                foreach (var transition in transitions)
                {
                    if (transition.CanTransition())
                    {
                        TrySwitchState(transition.ToState);

                        break;
                    }
                }
            }
        }
        
        _currentState?.Update();
    }

    public IntRect GetCurrentFrame()
    {
        return _currentState.GetCurrentFrameRect();
    }
    
    public Texture GetCurrentTexture()
    {
        return _currentState.GetCurrentSprite();
    }

    private void TrySwitchState(AnimationState state)
    {
        _currentState?.Exit();

        if (_currentState != null)
            StateChanged?.Invoke(_currentState);

        _currentState = state;
    }
}