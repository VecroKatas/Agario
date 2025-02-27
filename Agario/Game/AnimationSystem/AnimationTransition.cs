namespace Agario.Game.AnimationSystem;

public class AnimationTransition
{
    public AnimationState FromState { get; }
    public AnimationState ToState { get; }

    private readonly List<Func<bool>> _conditions = new();

    public AnimationTransition(AnimationState fromState, AnimationState toState)
    {
        FromState = fromState;
        ToState = toState;
    }

    public void AddCondition(Func<bool> condition)
    {
        _conditions.Add(condition);
    }

    public bool CanTransition()
    {
        foreach (var condition in _conditions)
        {
            if (!condition.Invoke())
            {
                return false;
            }
        }
        return true;
    }
}