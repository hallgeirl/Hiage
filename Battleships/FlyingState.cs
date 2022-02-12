
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
			particles = new ParticleEngine(gameref.Resources.GetTexture("particle"), 1000, gameref.Display.Renderer);
			
			System.Console.WriteLine("Initialized game state");
			player = new Ship(new Vector(0,0), gameref, new Vector(16,24));
			player.AddComponent(new PlayerShipBody(player, 0, 0));
			player.AddComponent(new SimpleGunTurret(player, 0, 0));
			
			testShip = new Ship(new Vector(100,100), gameref, new Vector(0,0));
			testShip.AddComponent(new PlayerShipBody(testShip, 0,0));
		}
		
		public void StartLevel(int level)
		{
			switch (level)
			{
			case 1:
				arena = new Arena(500, 500);
				foreach (Component obj in player.Components)
				{
					arena.AddObject(obj);
				}
				
			 	break;
			}
		}
		
		public void Run()
		{
			
			Random rnd = new Random();
			double magnitude = 0.1;
			double sinRot = Math.Sin(player.Rotation*Math.PI/180);
			double cosRot = Math.Cos(player.Rotation*Math.PI/180);
			Vector flamePos = new Vector(player.X + sinRot * 35, player.Y -10 - cosRot * 35);
			if (gameref.Input.KeyPressed(HKey.DownArrow))
			{
				player.Accellerate(-magnitude);
			}
			else if (gameref.Input.KeyPressed(HKey.UpArrow))
			{
				player.Accellerate(magnitude);
				Vector directionMod = new Vector(sinRot, -cosRot)*20;
				particles.SpawnParticle(20, flamePos, new Vector((rnd.NextDouble() - 0.5*Math.Abs(cosRot)), (rnd.NextDouble() - 0.5*Math.Abs(sinRot)))*10 + player.Velocity + directionMod, new Vector(0,0), 1, rnd.NextDouble(), 0, 1, true, 25+(int)(rnd.NextDouble()*10));
				particles.SpawnParticle(20, flamePos, new Vector((rnd.NextDouble() - 0.5*Math.Abs(cosRot)), (rnd.NextDouble() - 0.5*Math.Abs(sinRot)))*10 + player.Velocity + directionMod, new Vector(0,0), 1, rnd.NextDouble(), 0, 1, true, 10+(int)(rnd.NextDouble()*30));
			}
			else
			{
				player.Brake(0.05);
			}
			if (gameref.Input.KeyPressed(HKey.RightArrow))
			{
				player.Rotation-=2;
			}
			if (gameref.Input.KeyPressed(HKey.LeftArrow))
			{
				player.Rotation+=2;
			}

			particles.UpdateAndRender();

			player.Update();
			player.Render();
			

			testShip.Render();
			
			gameref.Display.Renderer.DrawLine(-50,0,50,0);
			gameref.Display.Renderer.DrawLine(0,-50,0,50);
			gameref.Display.Renderer.DrawLine(gameref.Display.CameraX,gameref.Display.CameraY,gameref.Display.CameraX+player.Velocity.X*10,gameref.Display.CameraY+player.Velocity.Y*10);
			gameref.Display.Renderer.DrawLine(player.X, player.Y, testShip.X, testShip.Y);
			gameref.Display.CameraX = player.X;
			gameref.Display.CameraY = player.Y;
		}
	}
}
