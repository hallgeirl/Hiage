
using System.Collections.Generic;
using System;

namespace Engine
{
	
	/// <summary>
	/// Grid used for spatial hashing
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
		}
		
		public void Add(T obj, double x, double y)
		{
			if (x >= 0 && y >= 0 && x < boundsX && y < boundsY)
			{
				squares[(int)(x/gridWidth), (int)(y/gridHeight)].Add(obj);
			}
			else
			{
				throw new IndexOutOfRangeException("(" + x + ", " + y + ") is out of range of the grid.");
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
		public void Move(T obj, double oldX, double oldY, double newX, double newY)
		{
			//Sanity check
			if (oldX < 0 || oldY < 0 || newX < 0 || newY < 0 || oldX >= boundsX || oldY >= boundsY || newX >= boundsX || newY >= boundsY)
			{
				throw new IndexOutOfRangeException("(" + oldX + ", " + oldY + ") or (" + newX + ", " + newY + ") is out of range of the grid.");
			}
			
			int oldSquareX = (int)(oldX / gridWidth);
			int oldSquareY = (int)(oldY / gridHeight);
			int newSquareX = (int)(newX / gridWidth);
			int newSquareY = (int)(newY / gridHeight);
			
			//Do we really need to move the object?
			if (oldSquareX == newSquareX && oldSquareY == newSquareY)
			{
				return;
			}
			
			//Ok, we need to move it. Move it!
			if (squares[oldSquareX, oldSquareY].Remove(obj))
			{
				//Successfully removed from the old square. Insert it again
				squares[newSquareX, newSquareY].Add(obj);
			}
		}
	}
}
