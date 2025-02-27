using Agario.Game.Configs;
using SFML.Graphics;

public class Animation
{
    public Sprite Sprite;
    public AnimationConfig Info;
    
    private int _gameFPS;
    public int TicksPerFrame;

    public Animation(Sprite sprite, AnimationConfig info)
    {
        _gameFPS = GameConfig.TargetFPS;
        
        Sprite = sprite;
        Info = info;
        TicksPerFrame = _gameFPS / Info.FramesPerSecond;

        SetFrame(info.StartFrame);
    }

    private void SetFrame(int frame)
    {
        Sprite.TextureRect = new IntRect(Info.StartingIntRectLeft + frame * Info.FrameWidth, Info.StartingIntRectTop, Info.FrameWidth, Info.FrameHeight);
    }

    public void UpdateFrame(int currentFrame)
    {
        SetFrame(Info.StartFrame + currentFrame);
    }
}