
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
		int cooldown;
		
		public SimpleGunTurret(Ship parent, double x, double y, Arena arena) : base(parent, x, y, false, arena, 16, 16)
		{
			turretSprite = new Sprite(parent.GameRef.Resources.GetSpriteDescriptor("turret"), parent.GameRef.Resources);
		}
		
		public override void Fire()
		{
			
			//Spawn a bullet
			if (cooldown <= 0)
			{
				Parent.Particles.SpawnParticle(3, new Vector(AbsoluteX, AbsoluteY), Parent.Velocity + Parent.Direction*10, new Vector(0,0), 1, 1, 0, 1, false, 20);
				Parent.Particles.SpawnParticle(3, new Vector(AbsoluteX, AbsoluteY), Parent.Velocity + Parent.Direction*10, new Vector(0,0), 1, 0.3, 0, 1, false, 10);
				
				//Smoke
				double grayShade = Rnd.Next();
				double smokeSpeed = 2;
				Vector baseVelocity = Parent.Velocity + Parent.Direction * 5;
				Vector accelVector = Parent.Direction * - 0.2;
				double smokeSize = 10;
				
				Parent.Particles.SpawnParticle(20, new Vector(AbsoluteX, AbsoluteY), baseVelocity + new Vector(Rnd.Next()*2-1, Rnd.Next()*2-1)*smokeSpeed, accelVector, grayShade, grayShade, grayShade, 1, false, (int)(smokeSize*Rnd.Next()));
				grayShade = Rnd.Next();
				Parent.Particles.SpawnParticle(20, new Vector(AbsoluteX, AbsoluteY), baseVelocity + new Vector(Rnd.Next()*2-1, Rnd.Next()*2-1)*smokeSpeed, accelVector, grayShade, grayShade, grayShade, 1, false, (int)(smokeSize*Rnd.Next()));
				grayShade = Rnd.Next();
				Parent.Particles.SpawnParticle(20, new Vector(AbsoluteX, AbsoluteY), baseVelocity + new Vector(Rnd.Next()*2-1, Rnd.Next()*2-1)*smokeSpeed, accelVector, grayShade, grayShade, grayShade, 1, false, (int)(smokeSize*Rnd.Next()));
				grayShade = Rnd.Next();
				Parent.Particles.SpawnParticle(20, new Vector(AbsoluteX, AbsoluteY), baseVelocity + new Vector(Rnd.Next()*2-1, Rnd.Next()*2-1)*smokeSpeed, accelVector, grayShade, grayShade, grayShade, 1, false, (int)(smokeSize*Rnd.Next()));
				grayShade = Rnd.Next();
				Parent.Particles.SpawnParticle(20, new Vector(AbsoluteX, AbsoluteY), baseVelocity + new Vector(Rnd.Next()*2-1, Rnd.Next()*2-1)*smokeSpeed, accelVector, grayShade, grayShade, grayShade, 1, false, (int)(smokeSize*Rnd.Next()));

				
				SimpleBullet bullet = new SimpleBullet(Parent, AbsoluteX, AbsoluteY, Parent.Rotation, 20 + Parent.Velocity.Length, ArenaRef);
				Parent.AddComponent(bullet);
				cooldown = 10;
			}
			
		}
		
		public override void Destroy()
		{
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
			turretSprite.Update(1);
			if (cooldown > 0)
				cooldown--;
		}
		
		public override void CollideWith(Component obj, Edge edge, double collX1, double collY1, double collX2, double collY2)
		{
		}
	}
	
}
