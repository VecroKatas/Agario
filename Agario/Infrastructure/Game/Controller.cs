using Agario.Game.AnimationSystem;

namespace Agario.Infrastructure;

public class Controller
{
    public GameObject TargetGameObject { get; protected set; }
    
    protected AnimatorBase _animator = null;
    
    public Controller(){}

    public Controller(GameObject targetGameObject)
    {
        TargetGameObject = targetGameObject;
    }

    public virtual void SetTargetGameObject(GameObject gameObject)
        => TargetGameObject = gameObject;

    public virtual void DestroyTargetGameObject()
    {
        TargetGameObject = null;
    }
}