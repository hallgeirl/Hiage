using System;
using Engine;

namespace Testing
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Game game = new Game();
			game.Initialize(800, 600, false);
			game.PushState(new TestState());
			game.MaxFPS = 60;
			game.Display.Zoom = 100;
			//game.Display.CameraX = game.Display.ViewportWidth / 2;
			//game.Display.CameraY = game.Display.ViewportHeight / 2;

			while (!game.Done)
			{
				game.Run();
			}
		}
	}
}