
using System;
using System.Collections.Generic;
using Engine;

namespace Battleships
{
	
	/// <summary>
	/// In-game game state (where you do the actual flying)
	/// </summary>
	public class FlyingState : IGameState
	{
		Game gameref;
		Ship player;//The player ship
		
		Ship testShip; //Someone else
		
		Arena arena;
		ParticleEngine particles;
		
		public FlyingState()
		{			
		}
		
		public void Initialize(Game game)
		{
			gameref = game;
			StartLevel(1);
			
			particles = new ParticleEngine(gameref.Resources.GetTexture("particle"), 10000, gameref.Display.Renderer);
			
			System.Console.WriteLine("Initialized game state");
			player = new Ship(new Vector(525,525), gameref, particles, arena);
			player.AddComponent(new PlayerShipBody(player, 0, 0, arena));
			player.AddComponent(new SimpleGunTurret(player, 16, -4, arena));
			player.AddComponent(new SimpleGunTurret(player, -16, -4, arena));
			player.AddComponent(new SimpleGunTurret(player, 0, -16, arena));
			player.AddComponent(new SimpleGunTurret(player, 0, 16, arena));
			player.AddComponent(new SimpleGunTurret(player, 24, -4, arena));
			player.AddComponent(new SimpleGunTurret(player, -24, -4, arena));
			
			testShip = new Ship(new Vector(400,400), gameref, particles, arena);
			testShip.AddComponent(new PlayerShipBody(testShip, 0,0, arena));
			testShip.AddComponent(new SimpleGunTurret(testShip, 40, -3, arena));
			testShip.AddComponent(new SimpleGunTurret(testShip, -40, -4, arena));
			
			player.AddExhaustPort(-8, -20, 30, 3, 50);
			player.AddExhaustPort(8, -20, 30, 3, 50);
			player.AddExhaustPort(0, -24, 30, 3, 50);
		}
		
		public void StartLevel(int level)
		{
			switch (level)
			{
			case 1:
				arena = new Arena(5000, 5000, gameref);
				/*foreach (Component obj in player.Components)
				{
					arena.AddObject(obj);
				}*/
				
			 	break;
			}
		}
		
		public void Run(double frameTime)
		{
			Renderer renderer = gameref.Display.Renderer;
			double magnitude = 7;
			double prod = 0;
			
			//Get input
			if (gameref.Input.KeyPressed(HKey.DownArrow))
			{
				player.Accellerate(-magnitude*frameTime);
			}
			else if (gameref.Input.KeyPressed(HKey.UpArrow))
			{
				prod = player.Velocity * player.Direction;
				prod = (prod<0?Math.Abs(prod/player.Velocity.Length)+1:1);
				player.Accellerate(magnitude*prod*frameTime);
				player.FireExhaust(2);
			}
			else
			{
				player.Brake(5.0*frameTime);
			}
			
			if (gameref.Input.KeyPressed(HKey.RightArrow))
			{
				player.Rotation-= 200*frameTime;
			}
			if (gameref.Input.KeyPressed(HKey.LeftArrow))
			{
				player.Rotation+= 200*frameTime;
			}
			
			if (gameref.Input.KeyPressed(HKey.LeftControl))
			{
				player.Fire();
			}

		
			player.Update();
			testShip.Update();
			arena.Update();
			
			arena.Render();

			//For debugging purposes: Highlight grid squares where the player is
			/*foreach (Component c in player.Components)
			{
				List<int> squares = arena.Grid.Locate(c);
				for (int i = 0; i < squares.Count; i+=2)
				{
					//Console.WriteLine(i);
					gameref.Display.Renderer.DrawSquare(squares[i]*arena.Grid.SquareWidth, 
					                                    squares[i+1]*arena.Grid.SquareHeight, 
					                                    (squares[i]+1)*arena.Grid.SquareWidth, 
					                                    (squares[i+1]+1)*arena.Grid.SquareHeight);
					//Console.WriteLine(squares[i]*arena.Grid.SquareWidth);
				}
				
			}
*/
			player.Render();
			testShip.Render();
			
			particles.UpdateAndRender(frameTime);						

			
			
			//Draw the text
			double left = gameref.Display.CameraX - gameref.Display.ViewportWidth / 2;
			double bottom = gameref.Display.CameraY - gameref.Display.ViewportHeight / 2;
			
			renderer.SetFont("font-arial", 12);
			renderer.SetDrawingColor(0,1,0,1);
			renderer.DrawText("Accel: " + Math.Round(magnitude*prod, 5), left, bottom+60);
			renderer.DrawText("FPS: " + Math.Round(gameref.FPS, 2), left, bottom+24);
			renderer.DrawText("Position: (" + Math.Round(player.X, 2) + ", " + Math.Round(player.Y, 2) + ")", left, bottom+36);
			renderer.DrawText("Speed: " + Math.Round(player.Velocity.Length, 2), left, bottom+48);
			renderer.SetDrawingColor(1,1,1,1);
			
			
			/*
			gameref.Display.Renderer.DrawLine(0,0,0,arena.Height);
			gameref.Display.Renderer.DrawLine(0,arena.Height, arena.Width, arena.Height);
			gameref.Display.Renderer.DrawLine(arena.Width,arena.Height, arena.Width, 0);
			gameref.Display.Renderer.DrawLine(arena.Width, 0, 0, 0);*/
			
			gameref.Display.CameraX = player.X;
			gameref.Display.CameraY = player.Y;

		}
	}
}
