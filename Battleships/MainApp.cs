
using System;
using Engine;
using SdlDotNet.Input;
using SdlDotNet;
using SdlDotNet.Core;

namespace Battleships
{
	
	
	public class MainApp
	{
		bool done = false;
		public MainApp()
		{
		}
		
		public void Start()
		{
			Game game = new Game();
			game.Initialize(640, 480, false);
			game.PushState(new FlyingState());
			game.MaxFPS = 60;
			game.Display.Zoom = 200;			
			
			int i = 0;
			Events.Quit += new EventHandler<QuitEventArgs>(this.OnQuit);
			while (!done)
			{
				while (Events.Poll()){}
				i++;
				//game.Display.Zoom += 0.1;
				if (i%60 == 0)
				{
					Console.WriteLine(game.FPS);
				}
				game.Run();
			}
		}
		
		public static void Main()
		{
			MainApp main = new MainApp();
			main.Start();
		}
		
		public void OnQuit(object sender, QuitEventArgs args)
		{
			done = true;
			Events.QuitApplication();
		}
	}
}
