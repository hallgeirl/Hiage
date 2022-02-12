
using System;
using Engine;

namespace Battleships
{
	
	
	public class MenuState : IGameState
	{
		
		public MenuState()
		{
		}
		
		public void Initialize(Game game)
		{
			Console.WriteLine("Initialized menu state");
		}
		
		public void Run()
		{
		}
	}
}
