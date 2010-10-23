using System;
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

			//For testing
			game.PushState(new LevelState(null, game, "level1"));
			//game.PushState(new LevelState(null, game, "test_multi"));
			
			while (!game.Done)
			{
				game.Run();
			}
		}
	}
}