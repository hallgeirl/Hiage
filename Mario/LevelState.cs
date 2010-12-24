using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	/*
	 *  Game state which handles the "playing" part of the game (tilemap, objects etc.)
	 */
	public class LevelState : IGameState
	{
		Game 				game;											 //Reference to the main game object				
		Display 			display;
		InputManager		input;
		
		ParallaxBackground	background = null;								 //Background used on this map
		TileMap 			tileMap;										 //The tiles used on this map
		List<GameObject> 	objects = new List<GameObject>();				 //All objects currently on this map
		ObjectFactory 		objectFactory;									 //Object factory used for creating objects
		Player 				player;											 //Reference to the player object
		WorldPhysics		worldPhysics = WorldPhysics.DefaultWorldPhysics; //Physics object used on this map. Attributes may be specified in the map file.		
		Timer 				fullscreenTimer = new Timer();	//Used to prevent a lot of fullscreen on/offs when holding down alt+enter more than one frame
		
		//Construct a levelstate object
		public LevelState (PlayerInfo player, Game game, string mapName)
		{
			this.game = game;
			this.display = game.Display;
			this.input = game.Input;
			
			game.Display.CameraX = 50;
			game.Display.CameraY = 100;
			
			//Load map
			MapDescriptor map = game.Resources.GetMapDescriptor(mapName);
			objectFactory = new ObjectFactory(game);
			
			tileMap = new TileMap(game.Display, game.Resources, map);
			
			//And set up the world physics attributes
			if (map.ExtraProperties.ContainsKey("gravity"))
				worldPhysics.Gravity = double.Parse(map.ExtraProperties["gravity"]);
			if (map.ExtraProperties.ContainsKey("ground-friction-factor"))
				worldPhysics.GroundFrictionFactor = double.Parse(map.ExtraProperties["ground-friction-factor"]);
			
			//Spawn all objects
			foreach (var o in map.Objects)
			{
				objects.Add(objectFactory.Spawn(o.Name, new Vector(o.X, o.Y), new Vector(0,-100), worldPhysics));
				if (o.Name == "mario")
				{
					this.player = (Player)objects[objects.Count-1];
				
				}
			}
			
			//Set the map background
			if (!string.IsNullOrEmpty(map.Background))
			    background = new ParallaxBackground(game.Resources.GetTexture(map.Background), 0.5, 0.2, game.Display);
		}
		
		public void Initialize(Game game)
		{
			game.Display.Renderer.SetFont("arial", 12);
		}
		
		//Simple insertion sort to sort all objects according to their left boundary
		private void SortObjects(double frameTime)
		{
			for (int i = 1; i < objects.Count; i++)
			{
				GameObject obj1 = objects[i];
				int j = i - 1;
				bool done = false;
				
				do
				{
					GameObject obj2 = objects[j];
					if (obj2.Left > obj1.Left)
					{
						objects[j+1] = objects[j];
						j--;
						if (j < 0)
							done = true;
					}
					else 
						done = true;
				} while (!done);
					
				objects[j+1] = obj1;
			}
		}
		
		//Perform all collision tests
		public void PerformCollisionTests(double frameTime)
		{
			
			SortObjects(frameTime);
			
			foreach (var o in objects)
			{
				//Log.Write(""+o.GetCollisionCheckArea(frameTime));
				CollisionManager.TestCollision(o, tileMap.GetBoundingPolygonsInRegion(o.GetCollisionCheckArea(frameTime), 0), frameTime);
			}
			//CollisionManager.PerformCollisionEvents();
			
			SortObjects(frameTime);
			
			for (int i = 0; i < objects.Count; i++)
			{
				for (int j = i+1; j < objects.Count && objects[i].Right >= objects[j].Left; j++)
				{
					CollisionManager.TestCollision(objects[i], objects[j], frameTime);
				}
				/*for (int j = i-1; j >= 0 && objects[j].Right >= objects[i].Left; j--)
				{
					CollisionManager.TestCollision(objects[i], objects[j], frameTime);
				}*/
			}
			
			CollisionManager.PerformCollisionEvents();
 		}

		public void Update(double frameTime)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Update(frameTime);
				if (objects[i].Delete)
				{
					objects.RemoveAt(i);
					if (i < objects.Count)
						i--;
				}
				
			}
			
			game.Display.CameraX = player.Position.X;
			game.Display.CameraY = player.Position.Y;
			
			
		}
		
		public void HandleInput(double frameTime)
		{
			if (game.Input.KeyPressed(HKey.LeftAlt) && game.Input.KeyPressed(HKey.Return))
			{
				if (!fullscreenTimer.Started || fullscreenTimer.Elapsed > 2000)
				{
					game.Display.Fullscreen = !game.Display.Fullscreen;
					fullscreenTimer.Start();
					fullscreenTimer.Restart();
				}
				
			}
		}
		
		public void Render(double frameTime)
		{
			if (background != null)
				background.Render();
			
			tileMap.Render(0);
			//tileMap.Render(1);

			foreach (GameObject o in objects)
			{
				o.Render(frameTime);
			}
			//tileMap.Render(2);
			
			game.Display.Renderer.DrawText("FPS: " + game.FPS.ToString("N2"), display.RenderedCameraX - display.ViewportWidth/2, display.RenderedCameraY - display.ViewportHeight/2);
		}
		
		public void Run(double frameTime)
		{
			foreach (var o in objects)
				o.Prepare(frameTime);
			
			HandleInput(frameTime);
			PerformCollisionTests(frameTime);
			
			Update(frameTime);
			
			Render(frameTime);
		}
	}
}
