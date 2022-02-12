
using Engine;
using System;

namespace Battleships
{
	
	/// <summary>
	/// Simple gun turret, firing a single bullet / shell. Must always be attached to a ship, so its x and y positions are always relative.
	/// </summary>
	public class SimpleGunTurret : Component
	{
		Sprite turretSprite;
		
		public SimpleGunTurret(Ship parent, double x, double y) : base(parent, x, y, false, false)
		{
			turretSprite = new Sprite(parent.GameRef.Resources.GetSpriteDescriptor("turret"), parent.GameRef.Resources);
		}
		
		public override void Fire(Vector direction)
		{
			Console.WriteLine("Gun turret is firing");
		}
		
		public override void Render()
		{
			turretSprite.X = AbsoluteX;
			turretSprite.Y = AbsoluteY;
			turretSprite.Rotation = Parent.Rotation;
			
			Parent.GameRef.Display.Renderer.Render(turretSprite);
		}
		
		public override void Update()
		{
			turretSprite.Update();
		}
	}
	
}
