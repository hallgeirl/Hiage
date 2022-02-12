
using Engine;

namespace Battleships
{
	
	
	public class Ship
	{
		Vector direction, position;
				
		public Ship(Vector pos, Vector dir)
		{
			direction = dir;
			position = pos;
		}
		
		public double X
		{
			get
			{
				return position.X;
			}
		}
		
		public double Y
		{
			get
			{
				return position.Y;
			}
		}
	}
}
