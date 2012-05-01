
using System;

namespace Battleships
{
	
	
	public class ComponentFactory
	{
		public static Component CreateComponent(string type, Ship parent, double x, double y, Arena arena)
		{
			switch (type)
			{
			case "SimpleGunTurret":
				return new SimpleGunTurret(parent, x, y, arena);
			default:
				return null;
			}
		}
	}
}
