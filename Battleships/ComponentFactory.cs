
using System;

namespace Battleships
{
	
	
	public class ComponentFactory
	{
		public static Component CreateComponent(string type, Ship parent, double x, double y)
		{
			switch (type)
			{
			case "SimpleGunTurret":
				return new SimpleGunTurret(parent, x, y);
			default:
				return null;
			}
		}
	}
}
