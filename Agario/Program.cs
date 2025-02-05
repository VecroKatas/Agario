using Agario.Game;
using Agario.Game.Configs;
using Agario.Infrastructure;

/*ConfigService.LoadConfig(typeof(GameConfig), "game.ini");
ConfigService.LoadConfig(typeof(PlayingMapConfig), "map.ini");
ConfigService.LoadConfig(typeof(GameObjectConfig), "gameobject.ini");
ConfigService.LoadConfig(typeof(PlayerConfig), "player.ini");*/

Boot boot = new Boot("entrypoint.ini");
boot.Launch(new AgarioGame());