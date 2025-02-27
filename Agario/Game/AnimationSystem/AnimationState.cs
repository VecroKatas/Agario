using SFML.Graphics;

namespace Agario.Game.AnimationSystem;

public class AnimationState
{
    public string Name;
    public Animation Animation;
    public bool IsLooped;

    public bool AnimationNotFinished = false;

    private int _currentFrame = 0;
    private int _ticksPassed = 0;

    public AnimationState(Animation animation, bool isLooped) : this(animation.Info.Name, animation, isLooped)
    { }
    
    public AnimationState(string stateName, Animation animation, bool isLooped)
    {
        Animation = animation;
        Name = stateName;
        IsLooped = isLooped;
    }

    public void Update()
    {
        _ticksPassed++;
        if (_ticksPassed >= Animation.TicksPerFrame || _currentFrame == 0)
        {
            AnimationNotFinished = true;
            
            _currentFrame = (_currentFrame + 1) % Animation.Info.FrameCount;
            _ticksPassed = 0;
                
            Animation.UpdateFrame(_currentFrame);

            if (_currentFrame == 0)
                AnimationNotFinished = false;
        }
    }

    public IntRect GetCurrentFrameRect()
    {
        return Animation.Sprite.TextureRect;
    }
    
    public Texture GetCurrentSprite()
    {
        return Animation.Sprite.Texture;
    }
    
    public void Exit()
    {
        _currentFrame = 0;
        _ticksPassed = 0;
    }
}