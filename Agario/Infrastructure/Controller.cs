using Agario.Game.Interfaces;

namespace Agario.Infrastructure;

public class Controller : IComponent
{
    public GameObject ParentGameObject { get; protected set; }
    public GameObject TargetGameObject { get; protected set; }
    
    public Controller(){}

    public Controller(GameObject parentGameObject)
    {
        ParentGameObject = parentGameObject;
    }

    public virtual void SetParentGameObject(GameObject gameObject)
        => ParentGameObject = gameObject;

    public virtual void SetTargetGameObject(GameObject targetGameObject)
        => TargetGameObject = targetGameObject;
}