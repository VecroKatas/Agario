using Agario.Game.Configs;
using SFML.Graphics;

class Animation
{
    public Sprite Sprite;
    public bool IsPlaying = false;

    private AnimationConfig _animationInfo;
    private int _gameFPS;
    private int _ticksPerFrame;
    private int _currentFrame = 0;
    private int _ticksPassed;

    public Animation(Sprite sprite, AnimationConfig animationInfo)
    {
        _gameFPS = GameConfig.TargetFPS;
        
        Sprite = sprite;
        _animationInfo = animationInfo;
        _ticksPerFrame = _gameFPS / _animationInfo.FramesPerSecond;

        SetFrame(animationInfo.StartFrame);
    }

    private void SetFrame(int frame)
    {
        Sprite.TextureRect = new IntRect(_animationInfo.StartingIntRectLeft + frame * _animationInfo.FrameWidth, _animationInfo.StartingIntRectTop, _animationInfo.FrameWidth, _animationInfo.FrameHeight);
    }

    public void Update()
    {
        _ticksPassed++;
        if (_ticksPassed >= _ticksPerFrame)
        {
            _currentFrame = (_currentFrame + 1) % _animationInfo.FrameCount;
            SetFrame(_animationInfo.StartFrame + _currentFrame);
            _ticksPassed = 0;
        }
    }

    public void Reset()
    {
        _currentFrame = 0;
        SetFrame(_animationInfo.StartFrame);
    }
}