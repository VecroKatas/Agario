using Agario.Game;
using Agario.Infrastructure;
using Agario.Infrastructure.Utilities;

GameConfig.Load(SolutionPathUtility.GetSolutionPath() + "\\config.ini");

Boot boot = new Boot(new AgarioGame());
boot.StartGame();