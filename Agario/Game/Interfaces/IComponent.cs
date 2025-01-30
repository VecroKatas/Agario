using Agario.Infrastructure;

namespace Agario.Game.Interfaces;

public interface IComponent
{
    public void SetParentGameObject(GameObject gameObject);
}