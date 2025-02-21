using Agario.Game.Configs;
using SFML.Graphics;

public struct Animation
{
    public Sprite Sprite;
    public AnimationConfig AnimationInfo;
    
    private int _gameFPS;
    public int TicksPerFrame;

    public Animation(Sprite sprite, AnimationConfig animationInfo)
    {
        _gameFPS = GameConfig.TargetFPS;
        
        Sprite = sprite;
        AnimationInfo = animationInfo;
        TicksPerFrame = _gameFPS / AnimationInfo.FramesPerSecond;

        SetFrame(animationInfo.StartFrame);
    }

    private void SetFrame(int frame)
    {
        Sprite.TextureRect = new IntRect(AnimationInfo.StartingIntRectLeft + frame * AnimationInfo.FrameWidth, AnimationInfo.StartingIntRectTop, AnimationInfo.FrameWidth, AnimationInfo.FrameHeight);
    }

    public void Update(int currentFrame)
    {
        SetFrame(AnimationInfo.StartFrame + currentFrame);
    }
}