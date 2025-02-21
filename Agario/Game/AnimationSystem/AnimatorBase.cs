using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.System;

namespace Agario.Game.AnimationSystem;

public class AnimatorBase : IComponent, IUpdatable
{
    private Animation currentAnimation;
    public GameObject GameObject;
    
    private int _currentFrame = 0;
    private int _ticksPassed;
    
    public AnimatorBase() 
    {
        GameCycle.GetInstance().RegisterObjectToUpdate(this);
    }
    
    public void SetParentGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
        GameObject.Shape.Scale = new Vector2f(2, 2);
    }
    
    public virtual void Play(string name)
    {
        if (AnimationsManager.Animations.ContainsKey(name))
        {
            if (currentAnimation.AnimationInfo == null || currentAnimation.AnimationInfo.Name != name)
            {
                GameObject.Shape.Texture = AnimationsManager.Animations[name].Sprite.Texture;
                currentAnimation = AnimationsManager.Animations[name];
                ResetCurrentAnimation();
            }
        }
    }

    public void Update()
    {
        if (currentAnimation.AnimationInfo != null)
        {
            _ticksPassed++;
            if (_ticksPassed >= currentAnimation.TicksPerFrame)
            {
                _currentFrame = (_currentFrame + 1) % currentAnimation.AnimationInfo.FrameCount;
                _ticksPassed = 0;
                
                currentAnimation.Update(_currentFrame);
                GameObject.Shape.TextureRect = currentAnimation.Sprite.TextureRect;
            }
        }
    }

    private void ResetCurrentAnimation()
    {
        _currentFrame = currentAnimation.AnimationInfo.StartFrame;
    }
}