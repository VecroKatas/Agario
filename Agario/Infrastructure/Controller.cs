using Agario.Game.Interfaces;

namespace Agario.Infrastructure;

public class Controller : IComponent
{
    public GameObject GameObject;
    
    public Controller(){}

    public Controller(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public virtual void SetGameObject(GameObject gameObject)
        => GameObject = gameObject;
}