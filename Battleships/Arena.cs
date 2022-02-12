using Engine;
using System;

namespace Battleships
{
	
	/// <summary>
	/// Battle arena (the map).
	/// </summary>
	public class Arena
	{
		private SpatialGrid<Component> grid;
		
		public Arena(int width, int height)
		{
			if (width < 1 || height < 1)
			{
				throw new InvalidOperationException("Cannot create an arena with width or height less than 1.");
			}
			
			//Create the arena grid, max grid size = 50
			grid = new SpatialGrid<Component>(width, height, 50);
		}
		
		public void AddObject(Component obj)
		{
			grid.Add(obj, obj.AbsoluteX, obj.AbsoluteY);
		}
	}
}
