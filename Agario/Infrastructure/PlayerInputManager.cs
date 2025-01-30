using SFML.Window;

namespace Agario.Infrastructure;

public enum KeyBindAction
{
    PlayerSwap,
}

public struct PlayerKeyMap
{
    public Dictionary<KeyBindAction, KeyBind> KeyBinds;
}

public class PlayerInputManager
{
    private Dictionary<KeyBindAction, KeyBind> keyBindings;
    
    public PlayerInputManager()
    {
        keyBindings = new Dictionary<KeyBindAction, KeyBind>();
    }

    public PlayerInputManager(PlayerKeyMap playerKeyMap)
    {
        keyBindings = new Dictionary<KeyBindAction, KeyBind>(playerKeyMap.KeyBinds);
    }

    public void SetKeyBinding(KeyBindAction actionName, Keyboard.Key key)
    {
        keyBindings[actionName] = new KeyBind(key);
    }
    
    public void AddOnDownKeyBinding(KeyBindAction actionName, Action action)
    {
        keyBindings[actionName].AddOnDownCallback(action);
    }
    
    public void ProcessInput()
    {
        foreach (var keyBind in keyBindings)
        {
            if (keyBind.Value.IsDown())
            {
                keyBind.Value.ProcessOnDownCallback();
            }
        }
    }
}