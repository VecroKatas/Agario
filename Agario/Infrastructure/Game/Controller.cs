namespace Agario.Infrastructure;

public class Controller
{
    public GameObject TargetGameObject { get; protected set; }
    
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