using SFML.System;

namespace Agario.Infrastructure;

public static class Time
{
    public static float DeltaTime { get; private set; }
    
    private static Clock _clock;
    private static float _totalTimeUntilUpdate = 0f;
    private static float _previousTotalTimeElapsed = 0f;
    private static float _totalTimeElapsed = 0f;

    public static void Start()
    {
        _clock = new Clock();
        DeltaTime = 0;
    }

    public static void Update()
    {
        _totalTimeElapsed = _clock.ElapsedTime.AsSeconds();
        float delta = _totalTimeElapsed - _previousTotalTimeElapsed;
        _previousTotalTimeElapsed = _totalTimeElapsed;

        _totalTimeUntilUpdate += delta;
    }

    public static bool IsNextUpdate()
    {
        return _totalTimeUntilUpdate >= GameCycle.TIME_UNTIL_NEXT_UPDATE;
    }

    public static void UpdateDeltaTime()
    {
        DeltaTime = _totalTimeUntilUpdate;
        _totalTimeUntilUpdate = 0f;
    }
}