namespace Agario.Infrastructure;

public class PlayerInputManager
{
    private readonly List<KeyBind> keyBindings;
    
    public PlayerInputManager()
    {
        keyBindings = new List<KeyBind>();
    }
    
    public void AddOnDownKeyBind(KeyBind keyBind, Action action)
    {
        int index = keyBindings.IndexOf(keyBind);
        keyBind.AddOnDownCallback(action);
        if (index != -1)
        {
            keyBindings[index] = keyBind;
        }
        else
        {
            keyBindings.Add(keyBind);
        }
    }
    
    public void ProcessInput()
    {
        foreach (var keyBind in keyBindings)
        {
            if (keyBind.IsDown())
            {
                keyBind.ProcessOnDownCallback();
            }
        }
    }
}