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
			game.MaxFPS = 0;
			game.Display.Zoom = 150;
			//Log.OutputStreamWriter = new StreamWriter("log.txt");

			PlayerState initialState = new PlayerState();
			game.PushState(new LevelState(initialState, game, "pipeland"));
			//game.PushState(new LevelState(null, game, "minimap"));
			//game.PushState(new LevelState(null, game, "test_multi"));
			
			while (!game.Done)
			{
				game.Run();
			}
		}
	}
}
