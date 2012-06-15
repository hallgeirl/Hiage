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
		Dictionary<string, GameObject> icons = new Dictionary<string, GameObject>();
		
		ParallaxBackground	background = null;								 //Background used on this map
		TileMap 			tileMap;										 //The tiles used on this map
		List<GameObject> 	objects = new List<GameObject>();				 //All objects currently on this map
		ObjectFactory 		objectFactory;									 //Object factory used for creating objects
		GameObject			player;											 //Reference to the player object
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
				//TODO: debug
//				if (!(o.Name == "mario")) continue;
				
				GameObject obj = objectFactory.Spawn(o.Name, new Vector(o.X, o.Y), new Vector(0,0), worldPhysics);
				if (obj != null)
				{
					objects.Add(obj);
					if (o.Name == "mario")
					{
						this.player = obj;
//						obj.AddComponent(new PlayerController(game.Input));
						//this.player = (Player)objects[objects.Count-1];
					}
				}
			}
			
			//Set the map background
			if (!string.IsNullOrEmpty(map.Background))
			    background = new ParallaxBackground(game.Resources.GetTexture(map.Background), 0.5, 0.2, game.Display);
			
			camera = new Camera(display, 0, map.Width * map.TileSize, 0, map.Height * map.TileSize);
			
			game.Audio.PlayMusic("overworld-intro", "overworld");
			
			icons["coin"] = objectFactory.Spawn("coin_icon", new Vector(0,0), new Vector(0,0), worldPhysics);
			icons["mario"] = objectFactory.Spawn("mario_icon", new Vector(0,0), new Vector(0,0), worldPhysics);
			//icons["coin"] = objectFactory.CreateIcon("coin", 0.7); //new Icon(game, Helpers.
			//icons["mario"] = objectFactory.CreateIcon("mario-big", 0.5); //new Icon(game, marioIcon);
		}

		//Simple insertion sort to sort all objects according to their left boundary
		private void SortObjects(double frameTime)
		{
			for (int i = 1; i < objects.Count; i++)
			{
				GameObject obj1 = objects[i];
				CollidableComponent go1 = (CollidableComponent)obj1.GetComponent("collidable");
				int j = i - 1;
				bool done = false;
				
				do
				{
					GameObject obj2 = objects[j];
					CollidableComponent go2 = (CollidableComponent)obj2.GetComponent("collidable");
					if (go1.Left < go2.Left)
					{
						objects[j+1] = objects[j];
						j--;
						if (j < 0)
							done = true;
					}
					else 
						done = true;
				} while (!done);
					
				//objects[j+1] = objects[i];
				objects[j+1] = obj1;
			}
			
			
		}
		
		//Perform all collision tests
		public void PerformCollisionTests(double frameTime)
		{
			
			SortObjects(frameTime);
			
//			for (int i = 0; i < objects.Count; i++)
//			{
//				GameObjectComponent go1 = (GameObjectComponent)objects[i].GetComponent("go");
//				if (!go1.CanCollide) continue;
//				for (int j = i+1; j < objects.Count; j++)
//				{
//					GameObjectComponent go2 = (GameObjectComponent)objects[j].GetComponent("go");
//					if (go1.Right < go2.Left) break;
//					if (!go2.CanCollide) continue;
//					CollisionManager.TestCollision(go1, go2, frameTime);
//				}
//			}
			
//			CollisionManager.PerformCollisionEvents();
 		}
		
		public void Update(double frameTime)
		{
			List<GOComponent> controllers = objectFactory.Components["controller"];
			List<GOComponent> gravity = objectFactory.Components["gravity"];
			List<GOComponent> friction = objectFactory.Components["friction"];
			List<GOComponent> motion = objectFactory.Components["motion"];
			List<GOComponent> collidable = objectFactory.Components["collidable"];
			List<GOComponent> collisionresponse = objectFactory.Components["collisionresponse"];
			List<GOComponent> transform = objectFactory.Components["transform"];
			List<GOComponent> statemachine = objectFactory.Components["statemachine"];
			List<GOComponent> speedlimit = objectFactory.Components["speedlimit"];
			
			foreach (ControllerComponent c in controllers)
				c.Update(frameTime);
			foreach (GravityComponent c in gravity)
				c.Update(frameTime);
			foreach (FrictionComponent c in friction)
				c.Update(frameTime);
			for (int axis = 0; axis <= 1; axis++)
			{
				foreach (MotionComponent c in motion)
					c.Update(frameTime, axis);
				foreach (SpeedLimitComponent c in speedlimit)
					c.Update(frameTime);
				foreach (TransformComponent c in transform)
					c.Update(frameTime, axis);
				foreach (CollidableComponent c in collidable)
				{
					c.Update(frameTime);
					c.TestCollision(tileMap.GetBoundingPolygonsInRegion(c.GetCollisionCheckArea(frameTime, axis), 0), frameTime, axis);
				}
				foreach (CollisionResponseComponent c in collisionresponse)
					c.Update(frameTime, axis);
			}
			foreach (StateMachineComponent s in statemachine)
				s.Update(frameTime);
			
			TransformComponent tr;
			tr = (TransformComponent)player.GetComponent("transform");
			camera.X = tr.Position.X;
			camera.Y = tr.Position.Y;
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
				DrawableComponent dc = (DrawableComponent)o.GetComponent("drawable");
				
				if (dc != null)
					dc.Update(frameTime);
			}
			
			for (int i = 2; i < tileMap.Layers; i++)
				tileMap.Render(i);
			
			//position and render icons
			double top = display.ViewTop, bottom = display.ViewBottom;
			double left = display.ViewLeft, right = display.ViewRight;
			TransformComponent trCoin = (TransformComponent)icons["coin"].GetComponent("transform");
			TransformComponent trMario = (TransformComponent)icons["mario"].GetComponent("transform");
			trCoin.Position.Set(right - 35, top-13);
			trMario.Position.Set(left + 20, top-13);
			
			foreach (GameObject o in icons.Values)
			{
				DrawableComponent dc = (DrawableComponent)o.GetComponent("drawable");
				if (dc != null)
					dc.Update(frameTime);
			}
			
			
			game.Display.Renderer.DrawText("FPS: " + game.FPS.ToString("N2"), left, bottom+4);
			game.Display.Renderer.DrawText("x" + playerInfo.Lives, left + 25, top - 16);
			game.Display.Renderer.DrawText("x" + playerInfo.Coins, right - 30, top - 16);
			
			/*GameObjectComponent coinIcon = (GameObjectComponent)icons["coin"].GetComponent("go");
			GameObjectComponent marioIcon = (GameObjectComponent)icons["mario"].GetComponent("go");*/

//			foreach (GameObject i in icons.Values)
//			{
//				DrawableComponent dc = (DrawableComponent)i.GetComponent("drawable");
//				RendererComponent r = (RendererComponent)i.GetComponent("renderer");
//				
//				dc.Update(frameTime);
//				if (r != null)
//					r.Update(frameTime);
//			}
			//game.Display.Renderer.DrawText("Player BB: " + objects[0].BoundingBox.ToString(), display.RenderedCameraX - display.ViewportWidth/2, display.RenderedCameraY - display.ViewportHeight/2 +16);
		}
		
		public override void Run(double frameTime)
		{
//			foreach (var o in objects)
//			{
//				GameObjectComponent go = (GameObjectComponent)o.GetComponent("go");
//				go.Prepare(frameTime);
//			}
			
			HandleInput(frameTime);
			
			Update(frameTime);
			
			PerformCollisionTests(frameTime);
			
			Render(frameTime);
		}
	}
}
