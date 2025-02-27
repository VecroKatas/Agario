using Agario.Game.Interfaces;

namespace Agario.Game.Configs;

public class AnimationConfig : IConfig
{
    public string Type;
    public string Name;
    public string FileName;
    public bool CanBeInterrupted;
    public int StartFrame;
    public int FrameCount;
    public int FramesPerSecond;
    public int FrameWidth;
    public int FrameHeight;
    public int StartingIntRectTop;
    public int StartingIntRectLeft;
}