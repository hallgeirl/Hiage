
using Engine;
using System;

namespace Battleships
{
	
	
	public class SimpleGunTurret : Component
	{
		Sprite turretSprite;
		
		public SimpleGunTurret(Ship parent, double x, double y) : base(parent, x, y)
		{
			
		}
		
		public override void Fire(Vector direction)
		{
			Console.WriteLine("Gun turret is firing");
		}
	}
}
