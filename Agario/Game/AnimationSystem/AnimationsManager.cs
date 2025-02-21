using Agario.Game.Configs;
using SFML.Graphics;

public class AnimationsManager
{
    public static Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
    private static string _animationFolderPath;
    
    public static void LoadAnimations()
    {
        _animationFolderPath = Path.Combine(AppContext.BaseDirectory, EntryPointConfig.AnimationsFolder);
        
        if (!Directory.Exists(_animationFolderPath))
        {
            Console.WriteLine($"[Animator] Animations folder '{_animationFolderPath}' not found.");
            return;
        }

        foreach (var pair in AnimationsConfig.Dictionary)
        {
            string fullPath = Path.Combine(_animationFolderPath, pair.Value.FileName);

            Animations[pair.Key] = new Animation(new Sprite(new Texture(fullPath)), pair.Value);
        }
    }
}