﻿using Agario.Game.Interfaces;
using SFML.Graphics;
using SFML.Window;

namespace Agario.Infrastructure;

public class Boot
{
    private RenderWindow _renderWindow;
    private IGameRules _gameRules;

    public Boot(IGameRules gameRules)
    {
        _renderWindow = new RenderWindow(new VideoMode(GameConfig.RenderWindowWidth, GameConfig.RenderWindowHeight), GameConfig.GameName);
        _gameRules = gameRules;
    }

    public void StartGame()
    {
        GameCycle gameCycleInstance = GameCycle.GetInstance();
        gameCycleInstance.Initialization(_renderWindow, _gameRules);
        gameCycleInstance.StartGameCycle();
    }
}