using Agario.Game.Interfaces;
using Agario.Infrastructure;
using SFML.System;

namespace Agario.Game.AnimationSystem;

public class AnimatorBase : IComponent, IUpdatable
{
    public GameObject GameObject;
    
    protected Animation _currentAnimation;
    protected bool _animationNotFinished = false;
    
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
            if (_currentAnimation.AnimationInfo == null || _currentAnimation.AnimationInfo.Name != name)
            {
                GameObject.Shape.Texture = AnimationsManager.Animations[name].Sprite.Texture;
                _currentAnimation = AnimationsManager.Animations[name];
                ResetCurrentAnimation();
            }
        }
    }

    public void Update()
    {
        if (_currentAnimation.AnimationInfo != null)
        {
            _ticksPassed++;
            if (_ticksPassed >= _currentAnimation.TicksPerFrame)
            {
                _animationNotFinished = true;
                _currentFrame = (_currentFrame + 1) % _currentAnimation.AnimationInfo.FrameCount;
                _ticksPassed = 0;
                
                _currentAnimation.Update(_currentFrame);
                GameObject.Shape.TextureRect = _currentAnimation.Sprite.TextureRect;

                if (_currentFrame == 0)
                    _animationNotFinished = false;
            }
        }
    }

    private void ResetCurrentAnimation()
    {
        _currentFrame = _currentAnimation.AnimationInfo.StartFrame;
    }
}