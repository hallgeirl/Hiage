using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	/*
	 *  Game state which handles the "playing" part of the game (tilemap, objects etc.)
	 */
	public class LevelState : MarioGameState
	{
		//Game 				game;											 //Reference to the main game object				
		Display 			display;
		Camera 				camera;
		PlayerState 		playerInfo;
		string				mapName;
		Dictionary<string, Icon> icons = new Dictionary<string, Icon>();
		
		ParallaxBackground	background = null;								 //Background used on this map
		TileMap 			tileMap;										 //The tiles used on this map
		List<GameObject> 	objects = new List<GameObject>();				 //All objects currently on this map
		ObjectFactory 		objectFactory;									 //Object factory used for creating objects
		Player 				player;											 //Reference to the player object
		WorldPhysics		worldPhysics = WorldPhysics.DefaultWorldPhysics; //Physics object used on this map. Attributes may be specified in the map file.		
		Timer 				fullscreenTimer = new Timer();	//Used to prevent a lot of fullscreen on/offs when holding down alt+enter more than one frame
		
		//Construct a levelstate object
		public LevelState (Game game, PlayerState player, string mapName) : base(game, player)
		{
			Activated += delegate {
				game.Display.Renderer.SetFont("verdana", 8);
			};

			playerInfo = player;
			
			this.game = game;
			this.display = game.Display;
			this.mapName = mapName;
//			game.Display.CameraX = 50;
			//game.Display.CameraY = 100;
			
			//Load map
			MapDescriptor map = game.Resources.GetMapDescriptor(mapName);
			objectFactory = new ObjectFactory(game, this);
			
			tileMap = new TileMap(game.Display, game.Resources, map);
			
			//And set up the world physics attributes
			if (map.ExtraProperties.ContainsKey("gravity"))
				worldPhysics.Gravity = double.Parse(map.ExtraProperties["gravity"]);
			if (map.ExtraProperties.ContainsKey("ground-friction-factor"))
				worldPhysics.GroundFrictionFactor = double.Parse(map.ExtraProperties["ground-friction-factor"]);
			
			//Spawn all objects
			foreach (var o in map.Objects)
			{
				GameObject obj = objectFactory.Spawn(o.Name, new Vector(o.X, o.Y), new Vector(0,-100), worldPhysics);
				if (obj != null)
				{
					objects.Add(obj);
					if (o.Name == "mario")
					{
						this.player = (Player)objects[objects.Count-1];
					}
				}
			}
			
			//Set the map background
			if (!string.IsNullOrEmpty(map.Background))
			    background = new ParallaxBackground(game.Resources.GetTexture(map.Background), 0.5, 0.2, game.Display);
			
			camera = new Camera(display, 0, map.Width * map.TileSize, 0, map.Height * map.TileSize);
			
			game.Audio.PlayMusic("overworld-intro", "overworld");
			
			icons["coin"] = objectFactory.CreateIcon("coin", 0.7); //new Icon(game, Helpers.
			icons["mario"] = objectFactory.CreateIcon("mario-big", 0.5); //new Icon(game, marioIcon);
			
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
			foreach (var o in objects)
			{
				if (!o.CanCollide) continue;
				CollisionManager.TestCollision(o, tileMap.GetBoundingPolygonsInRegion(o.GetCollisionCheckArea(frameTime), 0), frameTime);
			}
			
			SortObjects(frameTime);
			
			for (int i = 0; i < objects.Count; i++)
			{
				if (!objects[i].CanCollide) continue;
				for (int j = i+1; j < objects.Count && objects[i].Right >= objects[j].Left; j++)
				{
					if (!objects[j].CanCollide) continue;
					CollisionManager.TestCollision(objects[i], objects[j], frameTime);
				}
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
			
			camera.X = player.Position.X;
			camera.Y = player.Position.Y;
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
			
			for (int i = 0; i < Math.Min(tileMap.Layers, 2); i++)
				tileMap.Render(i);

			foreach (GameObject o in objects)
				o.Render(frameTime);
			
			for (int i = 2; i < tileMap.Layers; i++)
				tileMap.Render(i);
			
			double top = display.ViewTop, bottom = display.ViewBottom;
			double left = display.ViewLeft, right = display.ViewRight;
			game.Display.Renderer.DrawText("FPS: " + game.FPS.ToString("N2"), left, bottom+4);
			game.Display.Renderer.DrawText("x" + playerInfo.Lives, left + 25, top - 16);
			game.Display.Renderer.DrawText("x" + playerInfo.Coins, right - 30, top - 16);
			
			icons["coin"].Position = new Vector(right - 35, top-13);
			icons["mario"].Position = new Vector(left + 20, top-13);
			
			//render icons
			foreach (Icon i in icons.Values)
				i.Render(frameTime);
			
			//game.Display.Renderer.DrawText("Player BB: " + objects[0].BoundingBox.ToString(), display.RenderedCameraX - display.ViewportWidth/2, display.RenderedCameraY - display.ViewportHeight/2 +16);
		}
		
		public override void Run(double frameTime)
		{
			foreach (var o in objects)
				o.Prepare(frameTime);
			
			HandleInput(frameTime);
			Update(frameTime);
			
			PerformCollisionTests(frameTime);
			
			Render(frameTime);
		}
	}
}
