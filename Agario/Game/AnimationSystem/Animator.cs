using Agario.Game.Configs;
using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.Graphics;
using SFML.System;

class Animator : IComponent, IUpdatable
{
    private static Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
    private static string _animationFolderPath;
    
    private Animation currentAnimation;
    private Sprite sprite;

    private int frameWidth;
    private int frameHeight;

    public GameObject GameObject;

    public Animator() 
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
    }

    public void SetParentGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
        GameObject.Shape.Scale = new Vector2f(3, 3);
    }

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

            animations[pair.Key] = new Animation(new Sprite(new Texture(fullPath)), pair.Value);
        }
    }

    public void Play(string name)
    {
        if (animations.ContainsKey(name) && currentAnimation != animations[name])
        {
            GameObject.Shape.Texture = animations[name].Sprite.Texture;
            currentAnimation = animations[name];
            currentAnimation.Reset();
        }
    }

    public void Update()
    {
        currentAnimation?.Update();
        GameObject.Shape.TextureRect = currentAnimation.Sprite.TextureRect;
    }
}