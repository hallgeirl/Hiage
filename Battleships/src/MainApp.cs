
using System;
using Engine;


namespace Battleships
{
	
	
	public class MainApp
	{
		public MainApp()
		{
		}
		
		public void Start()
		{
			Game game = new Game();
			game.Initialize(800, 600, false);
			game.PushState(new FlyingState());
			game.Display.Zoom = 200;			

			while (!game.Done)
			{
				game.Run();
			}
		}
		
		public static void Main()
		{
			MainApp main = new MainApp();
			main.Start();
		}
	}
}
