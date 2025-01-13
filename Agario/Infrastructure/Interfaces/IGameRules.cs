using Agario.Infrastructure;

namespace Agario.Game.Interfaces;

public interface IGameRules : IInitializeable, IStartable, IPhysicsUpdatable, IUpdatable
{
    public PlayingMap PlayingMap { get; }
    public Action GameOver { get; set; }
}