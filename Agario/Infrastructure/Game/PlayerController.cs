namespace Agario.Infrastructure;

public class PlayerController : Controller
{
    public PlayerInputManager PlayerInputManager;

    public PlayerController() : base()
    {
        PlayerInputManager = new PlayerInputManager();
    }
}