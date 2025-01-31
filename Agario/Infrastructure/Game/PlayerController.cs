﻿using Agario.Game.Interfaces;

namespace Agario.Infrastructure;

public class PlayerController : Controller, IComponent
{
    public PlayerInputManager PlayerInputManager;

    public PlayerController() : base()
    {
        PlayerInputManager = new PlayerInputManager();
    }
}