using System;
using System.IO;
using Engine;

namespace Mario
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Game game = new Game();
			game.Initialize(800, 600, false, "Hiage Mario");
			game.MaxFPS = 60;
			game.Display.Zoom = 150;
			//Log.OutputStreamWriter = new StreamWriter("log.txt");

			PlayerState initialState = new PlayerState();
			initialState.HealthStatus = PlayerState.Health.Small;
			game.PushState(new LevelState(game, initialState, "level1"));
			//game.PushState(new LevelState(null, game, "minimap"));
			//game.PushState(new LevelState(null, game, "test_multi"));
			
			game.Run();
		}
	}
}
