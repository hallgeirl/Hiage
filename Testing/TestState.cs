
using System;
using System.Collections.Generic;
using Engine;

namespace Testing
{
	public class CollidableObject : ICollidable
	{		
		double width = 5, height = 5;
		int n = 0;
		Vector velocity;
		Vector position;
		
		public CollidableObject(double x, double y, Vector v)
		{
			position = new Vector(x, y);
			velocity = v;
			LastVelocity = new Vector(0,0);
			BoundingBox = new BoundingBox(position.X-width/2, position.Y+height/2, position.X+width/2, position.Y-height/2); 
		}
		
		public void Update(double frameTime)
		{
			//Update last position and last velocity
			LastVelocity.X = velocity.X;
			LastVelocity.Y = velocity.Y;
			position += velocity*frameTime;
			
			LastBoundingBox = (BoundingBox)BoundingBox.Clone();
			BoundingBox.Left = position.X-width/2;
			BoundingBox.Right = position.X+width/2;
			BoundingBox.Top = position.Y+height/2;
			BoundingBox.Bottom = position.Y-height/2;
		}
		
		public void Collide(ICollidable o, Vector edgeNormal, Vector penetration, double collisionTime)
		{
			position -= penetration*1.01;
			BoundingBox.Left = position.X-width/2;
			BoundingBox.Right = position.X+width/2;
			BoundingBox.Top = position.Y+height/2;
			BoundingBox.Bottom = position.Y-height/2;
			
			//Log.Write("Edge normal: " + edgeNormal);
			if (edgeNormal != null)
				velocity -= (2*velocity.DotProduct(edgeNormal))*edgeNormal;
			else
				Log.Write("Null edge normal.", Log.WARNING);
			//Velocity = new Vector(0,0);
		}
		
		public void Collide(Edge e, Vector penetration, double collisionTime)
		{
			//Log.Write("Collision at time " + collisionTime);
			n++;
			position -= penetration;
			
			velocity -= (2*velocity.DotProduct(e.Normal))*e.Normal;
			
			BoundingBox.Left = position.X-width/2;
			BoundingBox.Right = position.X+width/2;
			BoundingBox.Top = position.Y+height/2;
			BoundingBox.Bottom = position.Y-height/2;
		}

		public BoundingBox LastBoundingBox
		{
			get;
			private set;
		}
		
		public BoundingBox BoundingBox
		{
			get;
			private set;
		}

		public Vector LastVelocity
		{
			get;
			private set;
		}
		
	}
	
	public class TestState : IGameState
	{
		Game game;
		TileMap tilemap;
		//CollidableObject o1 = new CollidableObject(23, 3, new Vector(-10, 0)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		//CollidableObject o1 = new CollidableObject(23, -3, new Vector(-10, 0)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		//CollidableObject o1 = new CollidableObject(-3, 25, new Vector(0, -10)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		//CollidableObject o1 = new CollidableObject(3, 25, new Vector(0, -10)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		//CollidableObject o1 = new CollidableObject(-3, -25, new Vector(0, 10)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		//CollidableObject o1 = new CollidableObject(3, -25, new Vector(0, 10)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		//CollidableObject o1 = new CollidableObject(-23, 3, new Vector(10, 0)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		//CollidableObject o1 = new CollidableObject(-23, -3, new Vector(1000, 0)), o2 = new CollidableObject(0, 0, new Vector(0, 0));
		CollidableObject o1 = new CollidableObject(-10, -10, new Vector(-30, -70)), o2 = new CollidableObject(-30, -10, new Vector(75, 30));
		CollidableObject o3 = new CollidableObject(0,0,new Vector(0,0));
		//CollidableObject o1 = new CollidableObject(55, 53, new Vector(-30, -70)), o2 = new CollidableObject(-40, -30, new Vector(0, 0));
		
		//CollidableObject o1 = new CollidableObject(-20, 25, new Vector(0, -10)), o2 = new CollidableObject(-35, -25, new Vector(0, 0));
		
		public void Initialize(Game game)
		{
			this.game = game;
			tilemap = new TileMap(game.Display, game.Resources.GetTileset("grassland"), 10, 10, 10, -50, -50, 32);
			Log.Write("Edges: ");
			foreach (Edge e in tilemap.AllEdges)
			{
				Log.Write("" + e);
			}
			
		}
		Timer t = new Timer();
		
		public void Run(double frameTime)
		{
			if (!t.Started)
				t.Start();
			if (game.Input.KeyPressed(HKey.LeftAlt) && game.Input.KeyPressed(HKey.F) && t.Elapsed > 1)
			{
				game.Display.Fullscreen = !game.Display.Fullscreen;
				t.Reset();
				t.Start();
			}
			Renderer renderer = game.Display.Renderer;
			
			renderer.SetFont("arial", 12);
			
			if (game.GameTime > 1)
			{
				o1.Update(frameTime);
				o2.Update(frameTime);
				/*
				List<ICollidable> objects = new List<ICollidable>();
				objects.Add(o1);
				CollisionManager.TestCollision(o2, objects, frameTime);
	
				objects.Clear();
	
				objects.Add(o2);
				CollisionManager.TestCollision(o1, objects, frameTime);*/
	
				CollisionManager.TestCollision(o1, tilemap.AllEdges, frameTime);
				CollisionManager.TestCollision(o2, tilemap.AllEdges, frameTime);
				
				CollisionManager.PerformCollisionEvents();
			}
						
			tilemap.Render(0);
			renderer.DrawSquare(o1.BoundingBox.Left, o1.BoundingBox.Top, o1.BoundingBox.Right, o1.BoundingBox.Bottom);
			renderer.DrawSquare(o2.BoundingBox.Left, o2.BoundingBox.Top, o2.BoundingBox.Right, o2.BoundingBox.Bottom);
			renderer.DrawSquare(o3.BoundingBox.Left, o3.BoundingBox.Top, o3.BoundingBox.Right, o3.BoundingBox.Bottom);
			
			
			
			game.Resources.GetTileset("grassland");
			Vector mpos = game.Input.WindowToWorldPosition(game.Input.MouseWindowPosition, game.Display, false);
			game.Display.Renderer.DrawText("Mouse pos: (" + mpos.X + ", " + mpos.Y + ")", game.Display.CameraX - game.Display.ViewportWidth / 2, game.Display.CameraY - game.Display.ViewportHeight / 2);
		}
	}
}
