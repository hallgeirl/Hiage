using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
#if false
	
	/*
	 *  Game state which handles the "playing" part of the game (tilemap, objects etc.)
	 */
	public class DeadState : MarioGameState
	{
		Display 			display;
		Camera 				camera;
		PlayerState 		playerInfo;
		//string				mapName;
//		List<Icon>			icons;
		
		ParallaxBackground	background = null;								 //Background used on this map
		TileMap 			tileMap;										 //The tiles used on this map
		List<GameObject> 	objects = new List<GameObject>();				 //All objects currently on this map
		ObjectFactory 		objectFactory;									 //Object factory used for creating objects
		Player 				player;											 //Reference to the player object
		WorldPhysics		worldPhysics = WorldPhysics.DefaultWorldPhysics; //Physics object used on this map. Attributes may be specified in the map file.		
		Timer 				fullscreenTimer = new Timer();	//Used to prevent a lot of fullscreen on/offs when holding down alt+enter more than one frame
		
		//Construct a levelstate object
		public DeadState (Game game, PlayerState player, string mapName) : base(game, player)
		{
			Activated += delegate {
				game.Display.Renderer.SetFont("verdana", 8);
			};
			
			playerInfo = player;
			
			this.game = game;
			this.display = game.Display;
			//this.mapName = mapName;
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
				objects.Add(objectFactory.Spawn(o.Name, new Vector(o.X, o.Y), new Vector(0,-100), worldPhysics));
				if (o.Name == "mario")
				{
					this.player = (Player)objects[objects.Count-1].GetComponent("go");
				}
			}
			
			//Set the map background
			if (!string.IsNullOrEmpty(map.Background))
			    background = new ParallaxBackground(game.Resources.GetTexture(map.Background), 0.5, 0.2, game.Display);
			
			camera = new Camera(display, 0, map.Width * map.TileSize, 0, map.Height * map.TileSize);
			
			game.Audio.PlayMusic("overworld-intro", "overworld");
		}

		/*public override void Initialize(Game game)
		{
			
			//icons.Add(new Sprite(game.Resources.GetSpriteDescriptor(
		}*/
		
		//Simple insertion sort to sort all objects according to their left boundary
		private void SortObjects(double frameTime)
		{
			for (int i = 1; i < objects.Count; i++)
			{
				GameObjectComponent obj1 = (GameObjectComponent)objects[i].GetComponent("go");
				int j = i - 1;
				bool done = false;
				
				do
				{
					GameObjectComponent obj2 = (GameObjectComponent)objects[j].GetComponent("go");
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
					
				objects[j+1] = objects[i];
			}
		}
		
		//Perform all collision tests
		public void PerformCollisionTests(double frameTime)
		{
			foreach (var o in objects)
			{
				GameObjectComponent go = (GameObjectComponent)o.GetComponent("go");
				//CollisionManager.TestCollision(go, tileMap.GetBoundingPolygonsInRegion(go.GetCollisionCheckArea(frameTime), 0), frameTime);
				CollidableComponent cc = (CollidableComponent)o.GetComponent("collidable");
				if (cc != null)
				{
					cc.TestCollision(tileMap.GetBoundingPolygonsInRegion(go.GetCollisionCheckArea(frameTime), 0), frameTime);
				}
			}
			
			SortObjects(frameTime);
			
			for (int i = 0; i < objects.Count; i++)
			{
				GameObjectComponent go1 = (GameObjectComponent)objects[i].GetComponent("go");
				for (int j = i+1; j < objects.Count; j++)
				{
					GameObjectComponent go2 = (GameObjectComponent)objects[j].GetComponent("go");
					if (go1.Right >= go2.Left) break;
					CollisionManager.TestCollision(go1, go2, frameTime);
				}
			}
			
			CollisionManager.PerformCollisionEvents();
 		}

		public void Update(double frameTime)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				GameObjectComponent go = (GameObjectComponent)objects[i].GetComponent("go");
				go.Update(frameTime);
				if (go.Delete)
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
			{
			}
			
			for (int i = 2; i < tileMap.Layers; i++)
				tileMap.Render(i);
			
			game.Display.Renderer.DrawText("FPS: " + game.FPS.ToString("N2"), display.RenderedCameraX - display.ViewportWidth/2, display.RenderedCameraY - display.ViewportHeight/2+4);
			game.Display.Renderer.DrawText("Lives: " + playerInfo.Lives, display.RenderedCameraX - display.ViewportWidth/2 + 5, display.RenderedCameraY + display.ViewportHeight/2 - 16);
			game.Display.Renderer.DrawText("Coins: " + playerInfo.Coins, display.RenderedCameraX + display.ViewportWidth/2 - 55, display.RenderedCameraY + display.ViewportHeight/2 - 16);
			
			//game.Display.Renderer.DrawText("Player BB: " + objects[0].BoundingBox.ToString(), display.RenderedCameraX - display.ViewportWidth/2, display.RenderedCameraY - display.ViewportHeight/2 +16);
		}
		
		public override void Run(double frameTime)
		{
			foreach (var o in objects)
			{
				GameObjectComponent go = (GameObjectComponent)o.GetComponent("go");
				
				go.Prepare(frameTime);
			}
			
			HandleInput(frameTime);
			Update(frameTime);
			
			PerformCollisionTests(frameTime);
			
			Render(frameTime);
		}
	}
#endif
}
