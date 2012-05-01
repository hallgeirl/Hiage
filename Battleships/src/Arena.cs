using Engine;
using System;
using System.Collections.Generic;

namespace Battleships
{

	public enum Edge
	{
		LEFT = 1,
		BOTTOM = 2,
		RIGHT = 3,
		TOP = 4
	}
	
	
	/// <summary>
	/// Battle arena (the map).
	/// </summary>
	public class Arena
	{
		//Class used to schedule collision tests
		private class CollisionTest
		{
			public Component object1, object2;
			
			public CollisionTest(Component obj1, Component obj2)
			{
				object1 = obj1;
				object2 = obj2;
			}
		}
		
		private SpatialGrid<Component> grid;
		int width, height;
		ParallaxBackground background;
		Game gameref;
		
		//List<Ship> ships;
		List<CollisionTest> collisions = new List<CollisionTest>();
		
		public Arena(int width, int height, Game game)
		{
			if (width < 1 || height < 1)
			{
				throw new InvalidOperationException("Cannot create an arena with width or height less than 1.");
			}
			
			this.width = width;
			this.height = height;
			gameref = game;
			
			//Create the arena grid, max grid size = 50
			grid = new SpatialGrid<Component>(width, height, 75);
			
			background = new ParallaxBackground(game.Resources.GetTexture("starscape1"), 0.3, game.Display);
		}
		
		public void AddObject(Component obj)
		{
			grid.Add(obj, obj.AbsoluteX, obj.AbsoluteY, obj.Width, obj.Height);
		}
		
		public void MoveObject(Component obj, double oldX, double oldY, double newX, double newY, int oldWidth, int oldHeight, int newWidth, int newHeight)
		{
			grid.Move(obj, oldX, oldY, newX, newY, oldWidth, oldHeight, newWidth, newHeight);
		}
		
		public void AddCollisionTest(Component obj1, Component obj2)
		{
			//If object 1 collides with object 2, then also object 2 collides with object 1. Only add one of these instances.
			if (obj1.ID < obj2.ID)
			{
				collisions.Add(new CollisionTest(obj1, obj2));
			}
		}
		
		public void Update()
		{
			//Console.WriteLine("---");
			//Perform collision tests
			foreach (CollisionTest test in collisions)
			{
				Component o1 = test.object1;
				Component o2 = test.object2;
				
				//Calculate velocities based on the old position and new position of the objects
				Vector v1 = new Vector(o1.AbsoluteX - o1.OldX, o1.AbsoluteY - o1.OldY);
				Vector v2 = new Vector(o2.AbsoluteX - o2.OldX, o2.AbsoluteY - o2.OldY);

				Vector vDist = v1 - v2; //Distance change vector

				//First, check if object1 enters the boundaries of object2
				
				//Check collision between Bottom1 and Top2
				double t1 = (o1.Bottom-o2.Top)/vDist.Y;
				//And Top1 and Bottom2
				double t2 = (o1.Top-o2.Bottom)/vDist.Y;
				//Right1 and Left2
				double t3 = (o1.Right-o2.Left)/vDist.X;
				//Left1 and Right2
				double t4 = (o1.Left-o2.Right)/vDist.X;
				
				double collisionTime = 10;
				Edge collisionEdge = Edge.LEFT;
				
				bool enterFromTop = (t1 >= 0 && t1 < 1);
				bool enterFromBottom = (t2 >= 0 && t2 < 1);
				bool enterFromLeft = (t3 >= 0 && t3 < 1);
				bool enterFromRight = (t4 >= 0 && t4 < 1);
				
				if (enterFromTop && (enterFromLeft || enterFromRight || (o1.Left < o2.Right && o1.Right > o2.Left)))
				{
					collisionTime = t1;
					collisionEdge = Edge.BOTTOM;
				}
				if (enterFromBottom && (enterFromLeft || enterFromRight || (o1.Left < o2.Right && o1.Right > o2.Left)))
				{
					if (t2 < collisionTime)
					{
						collisionTime = t2;
						collisionEdge = Edge.TOP;
					}
				}
				if (enterFromLeft && (enterFromTop || enterFromBottom || (o1.Bottom < o2.Top && o1.Top > o2.Bottom)))
				{
					if (t3 < collisionTime)
					{
						collisionTime = t3;
						collisionEdge = Edge.RIGHT;
					}
				}
				if (enterFromRight && (enterFromTop || enterFromBottom || (o1.Bottom < o2.Top && o1.Top > o2.Bottom)))
				{
					if (t4 < collisionTime)
					{
						collisionTime = t4;
						collisionEdge = Edge.LEFT;
					}
				}	
				
				if (collisionTime >= 0 && collisionTime < 1)
				{
					//Send a message to the objects telling them about the collision
					o1.CollideWith(o2, collisionEdge, 
					               //o1.OldX + v1.X * collisionTime, o1.OldY + v1.Y * collisionTime, 
					               //o2.OldX + v2.X * collisionTime, o2.OldY + v2.Y * collisionTime);
					               o1.AbsoluteX - v1.X * collisionTime, o1.OldY + v1.Y * collisionTime, 
					               o2.AbsoluteX - v2.X * collisionTime, o2.OldY + v2.Y * collisionTime);


					Edge oppositeEdge = Edge.LEFT;
					switch (collisionEdge)
					{
					case Edge.TOP:
						oppositeEdge = Edge.BOTTOM;
						break;
					case Edge.BOTTOM:
						oppositeEdge = Edge.TOP;
						break;
					case Edge.LEFT:
						oppositeEdge = Edge.RIGHT;
						break;
					case Edge.RIGHT:
						oppositeEdge = Edge.LEFT;
						break;
					}
					o2.CollideWith(o1, oppositeEdge, 
//					               o2.OldX + v2.X * collisionTime, o2.OldY + v2.Y * collisionTime, 
//					               o1.OldX + v1.X * collisionTime, o1.OldY + v1.Y * collisionTime);
					               o2.AbsoluteX - v2.X * collisionTime, o2.OldY + v2.Y * collisionTime, 
					               o1.AbsoluteX - v1.X * collisionTime, o1.OldY + v1.Y * collisionTime);
				}

				
				//Check if the squares overlap
				//if (o1.Left < o2.Right && o1.Right > o2.Left && o1.Top > o2.Bottom && o1.Bottom < o2.Top)
				//{
					//Console.WriteLine("COLLISION between " + o1.ID + " and " + o2.ID);
				//}
				
				
				
				//Console.WriteLine("Coll between " + test.object1.ID + " and " + test.object2.ID);
			}
			collisions.Clear();
		}
		
		public void Render()
		{
			background.Render();
			
			//Render a grid (debugging)
			//TODO: Remove this
			/*for (int x = 0; x < grid.SquaresX; x++)
			{
				gameref.Display.Renderer.DrawLine(x*grid.SquareWidth, 0, x*grid.SquareWidth, grid.SquaresY*grid.SquareHeight);
			}
			for (int y = 0; y < grid.SquaresY; y++)
			{
				gameref.Display.Renderer.DrawLine(0, y*grid.SquareHeight, grid.SquaresX*grid.SquareWidth, y*grid.SquareHeight);				
			}*/
			gameref.Display.Renderer.DrawLine(0, 0, grid.SquaresX*grid.SquareWidth, 0);
			gameref.Display.Renderer.DrawLine(0, grid.SquaresY*grid.SquareHeight, grid.SquaresX*grid.SquareWidth, grid.SquaresY*grid.SquareHeight);
			gameref.Display.Renderer.DrawLine(0, 0, 0, grid.SquaresY*grid.SquareHeight);
			gameref.Display.Renderer.DrawLine(grid.SquaresX*grid.SquareWidth, 0, grid.SquaresX*grid.SquareWidth, grid.SquaresY*grid.SquareHeight);
		}

		
		public int Height
		{
			get
			{
				return height;
			}
		}
		
		public int Width
		{
			get
			{
				return width;
			}
		}
		
		
		//TODO: Remove this.
		public SpatialGrid<Component> Grid
		{
			get
			{
				return grid;
			}
			
		}
	}
}
