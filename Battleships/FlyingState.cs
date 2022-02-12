
using System;
using Engine;

namespace Battleships
{
	
	/// <summary>
	/// In-game game state (where you do the actual flying)
	/// </summary>
	public class FlyingState : IGameState
	{
		Game gameref;
		Sprite testSprite;
		Ship ship = new Ship(new Vector(0,0), new Vector(0,1));

		public FlyingState()
		{			
		}
		
		public void Initialize(Game game)
		{
			gameref = game;
			System.Console.WriteLine("Initialized game state");
			testSprite = new Sprite(gameref.Resources.GetSpriteDescriptor("testsprite"), gameref.Resources);
			testSprite.PlayAnimation("active");
		}
		
		public void StartLevel(int level)
		{
			
		}
		
		public void Run()
		{
			if (gameref.Input.KeyPressed(HKey.DownArrow))
			{
				testSprite.Y--;
			}
			testSprite.Update();
			gameref.Display.Renderer.Render(testSprite);
			
		}
	}
}
