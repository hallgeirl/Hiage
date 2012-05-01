
using System.Collections.Generic;
using System;

namespace Engine
{
	
	/// <summary>
	/// Grid used for spatial hashing.
	/// </summary>
	public class SpatialGrid<T>
	{
		private class GridSquare
		{
			//TODO: Create a binary search tree for the object list
			List<T> objects = new List<T>();
			
			/// <summary>
			/// Add a new object to this spatial grid square.
			/// </summary>
			/// <param name="obj">
			/// A <see cref="T"/>
			/// </param>
			public void Add(T obj)
			{
				objects.Add(obj);
			}
			
			/// <summary>
			/// Remove an object.
			/// </summary>
			/// <param name="obj">
			/// A <see cref="T"/>
			/// </param>
			public bool Remove(T obj)
			{
				return objects.Remove(obj);
			}
			
			public bool Find(T obj)
			{
				foreach (T o in objects)
				{
					if (object.ReferenceEquals(o, obj))
					{
						return true;
					}
				}
				return false;
			}
			
			public List<T> Objects
			{
				get
				{
					return objects;
				}
			}
		}
		
		//All squares in the grid
		GridSquare [,] squares;

		//Number of squares in the X and Y direction
		int squaresX, squaresY;
		
		//Width and height of squares
		int gridWidth, gridHeight;
		
		//Outer bounds of map (A little redundant, but used for simplicity and to avoid unneccesary calculations)
		int boundsX, boundsY;
		
		/// <summary>
		/// Construct a spatial grid object.
		/// </summary>
		/// <param name="width">
		/// A <see cref="System.Int32"/>. Width of the grid.
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>. Height of the grid.
		/// </param>
		/// <param name="gridsize">
		/// A <see cref="System.Int32"/>. Maximum allowed size of grid squares/rectangles.
		/// </param>
		public SpatialGrid(int width, int height, int gridsize)
		{
			//How many grid squares do we need?
			squaresX = (int)Math.Ceiling((double)width/(double)gridsize);
			squaresY = (int)Math.Ceiling((double)height/(double)gridsize);
			
			//How large should the grid squares be?
			gridWidth = width / squaresX;
			gridHeight = height / squaresY;
			
			//Calculate outer bounds
			boundsX = gridWidth * squaresX;
			boundsY = gridHeight * squaresY;
			
			//Create the array containing all the squares
			squares = new GridSquare[squaresX, squaresY];
			for (int x = 0; x < squaresX; x++)
			{
				for (int y = 0; y < squaresY; y++)
				{
					squares[x,y] = new GridSquare();
				}
			}
		}
		
		public void Add(T obj, double x, double y, double width, double height)
		{
			if (x >= 0 && y >= 0 && x < boundsX && y < boundsY)
			{
				//Which squares are needed?
				int left = (int)((x-width/2)/gridWidth);
				int top = (int)((y+height/2)/gridHeight);
				int right = (int)((x+width/2)/gridWidth);
				int bottom = (int)((y-height/2)/gridHeight);
				
				if (left < 0 || right >= squaresX || top >= squaresY || bottom < 0)
				{
					throw new IndexOutOfRangeException("(" + x + ", " + y + ") with dimensions " + width + "x" + height + " is out of range of the grid.");
				}
				
				for (int xi = left; xi <= right; xi++)
				{
					for (int yi = bottom; yi <= top; yi++)
					{
						squares[xi, yi].Add(obj);
					}
				}
				
			}
			else
			{
				throw new IndexOutOfRangeException("(" + x + ", " + y + ") is out of range of the grid.");
			}
		}
		
		public void Remove(T obj, double x, double y, int width, int height)
		{
			int left = (int)((x-width/2)/gridWidth);
			int top = (int)((y+height/2)/gridHeight);
			int right = (int)((x+width/2)/gridWidth);
			int bottom = (int)((y-height/2)/gridHeight);
			
			if (left < 0 || right >= squaresX || top >= squaresY || bottom < 0)
			{
				throw new IndexOutOfRangeException("(" + x + ", " + y + ") with dimensions " + width + "x" + height + " is out of range of the grid.");
			}
			
			for (int xi = left; xi <= right; xi++)
			{
				for (int yi = bottom; yi <= top; yi++)
				{
					squares[xi, yi].Remove(obj);
				}
			}			
		}
		
		/// <summary>
		/// Move an object from the old grid square to a new one
		/// </summary>
		/// <param name="obj">
		/// A <see cref="T"/>
		/// </param>
		/// <param name="oldX">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="oldY">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="newX">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="newY">
		/// A <see cref="System.Int32"/>
		/// </param>
		public void Move(T obj, double oldX, double oldY, double newX, double newY, int oldWidth, int oldHeight, int newWidth, int newHeight)
		{
			//Which squares are needed?
			int oldLeft = (int)(Math.Floor((oldX-oldWidth/2)/gridWidth));
			int oldTop = (int)((oldY+oldHeight/2)/gridHeight);
			int oldRight = (int)((oldX+oldWidth/2)/gridWidth);
			int oldBottom = (int)(Math.Floor((oldY-oldHeight/2)/gridHeight));

			int newLeft = (int)((newX-newWidth/2)/gridWidth);
			int newTop = (int)((newY+newHeight/2)/gridHeight);
			int newRight = (int)((newX+newWidth/2)/gridWidth);
			int newBottom = (int)((newY-newHeight/2)/gridHeight);
			
			//Sanity check
			if (oldLeft < 0 || oldBottom < 0 || newLeft < 0 || newBottom < 0 || oldRight >= squaresX || oldTop >= squaresY || newRight >= squaresX || newTop >= squaresY)
			{
				throw new IndexOutOfRangeException("(" + oldX + ", " + oldY + ") or (" + newX + ", " + newY + ") is out of range of the grid.");
			}
			
			//Do we really need to move the object?
			if (oldLeft == newLeft && oldTop == newTop && oldRight == newRight && oldBottom == newBottom)
			{
				return;
			}
			
			//Console.WriteLine("Moving " + obj + " from " + oldX + ", " + oldY + " to " + newX + ", " + newY);
			//Ok, we need to move it. Move it!
			for (int xi = oldLeft; xi <= oldRight; xi++)
			{
				for (int yi = oldBottom; yi <= oldTop; yi++)
				{
					if (!squares[xi, yi].Remove(obj))
					{
						Log.Write("SpatialGrid.Move: Object " + obj + " not found in square (" + xi + ", " + yi + ")", Log.WARNING);
					}
				}
			}
			
			for (int xi = newLeft; xi <= newRight; xi++)
			{
				for (int yi = newBottom; yi <= newTop; yi++)
				{
					squares[xi, yi].Add(obj);
				}
			}
		}
				
		/// <summary>
		/// Find the squares containing a specified object
		/// </summary>
		/// <param name="obj">
		/// A <see cref="T"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List<int> Locate(T obj)
		{
			List<int> l = new List<int>();
			for (int x = 0; x < squaresX; x++)
			{
				for (int y = 0; y < squaresY; y++)
				{
					if (squares[x,y].Find(obj))
					{
						l.Add(x);
						l.Add(y);
					}
				}
			}
			return l;
		}
		
		/// <summary>
		/// Retrieve all objects from the squares who are overlapped by the specified area. 
		/// This function uses SortedList for temporarily storing the objects, and it uses the GetHashCode() function as key. 
		/// Therefore, GetHashCode() needs to be overridden in the class used with the SpatialGrid class to return an unique value.
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Double"/> specifying the X position of the center of the area to search
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Double"/> specifying the Y position
		/// </param>
		/// <param name="width">
		/// A <see cref="System.Int32"/>. The width of the area to search.
		/// </param>
		/// <param name="height">
		/// A <see cref="System.Int32"/>. The height.
		/// </param>
		/// <param name="radius">
		/// A <see cref="System.Int32"/> specifying how much further (in squares) to expand the search. A common value is 1.
		/// </param>
		/// <returns>
		/// A <see cref="List"/> containing all objects found in the squares searched
		/// </returns>
		public IList<T> GetSurroundingObjects(double x, double y, int width, int height, int radius)
		{
			if (width < 1 || height < 1)
			{
				throw new ArgumentOutOfRangeException("GetSurroundingObjects: Non-positive dimensions is not allowed.");
			}
			//Calculate edges of collected squares
			int left = ((int)x-width/2)/gridWidth;
			int right = ((int)x+width/2)/gridWidth;
			int top = ((int)y+height/2)/gridHeight;
			int bottom = ((int)y-height/2)/gridHeight;
			
			if (left < 0 || right >= squaresX || top >= squaresY || bottom < 0)
			{
				throw new IndexOutOfRangeException("(" + x + ", " + y + ") with dimensions " + width + "x" + height + " is out of range of the grid.");
			}
			
			//Expand the edges with radius, if possible
			left = Math.Max(0, left - radius);
			right = Math.Min(squaresX-1, right+radius);
			top = Math.Min(squaresY-1, top+radius);
			bottom = Math.Max(0, bottom - radius);

			//Put all values in a sorted list, sorted on GetHashCode().
			SortedList<int, T> objects = new SortedList<int, T>();

			for (int xi = left; xi <= right; xi++)
			{
				for (int yi = bottom; yi <= top; yi++)
				{
					List<T> tempObjects = squares[xi,yi].Objects;
					
					foreach (T obj in tempObjects)
					{
						if (!objects.ContainsKey(obj.GetHashCode()))
						{
							objects.Add(obj.GetHashCode(), obj);
						}
					}
				}
			}
			return objects.Values;
			
		}
		
		public int SquareWidth
		{
			get
			{
				return gridWidth;
			}
		}
		
		public int SquareHeight
		{
			get
			{
				return gridHeight;
			}
		}
		
		public int SquaresX
		{
			get
			{
				return squaresX;
			}
		}

		public int SquaresY
		{
			get
			{
				return squaresY;
			}
		}				
	}
}
