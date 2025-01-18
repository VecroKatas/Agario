using Agario.Infrastructure;
using SFML.Graphics;

namespace Agario.Game.Interfaces;

public interface IGameRules : IInitializeable, IPhysicsUpdatable, IUpdatable
{
    public PlayingMap PlayingMap { get; }
    public Action GameRestart { get; set; }

    public GameObject GetGameObjectToFocusOn();
    public List<GameObject> GetGameObjectsToDisplay();
    public List<Text> GetTextsToDisplay();
}