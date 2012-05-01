
using System;
using Engine;

namespace Battleships
{
	/// <summary>
	/// Simple gun bullet.
	/// </summary>
	public class SimpleBullet : Component
	{
		Sprite bulletSprite;
		double angle, magnitude;
		
		public SimpleBullet(Ship parent, double x, double y, double angle, double magnitude, Arena arena) : base(parent, x, y, true, arena, 4, 4)
		{
			bulletSprite = new Sprite(parent.GameRef.Resources.GetSpriteDescriptor("bullet"), parent.GameRef.Resources);
			this.angle = angle;
			this.magnitude = magnitude;
		}
		
		public override void Fire()
		{
			
		}
		
		public override void Destroy()
		{
			Parent.Particles.SpawnParticle(5, new Vector(AbsoluteX, AbsoluteY), new Vector(Rnd.Next()*2-1, Rnd.Next()*2-1)*1, new Vector(0,0), 1, Rnd.Next(), 0, 1, false, 10);
			Parent.Particles.SpawnParticle(5, new Vector(AbsoluteX, AbsoluteY), new Vector(Rnd.Next()*2-1, Rnd.Next()*2-1)*1, new Vector(0,0), 1, Rnd.Next(), 0, 1, false, 10);
			Parent.RemoveComponent(this);
		}
		
		public override void Render()
		{
			bulletSprite.X = AbsoluteX;
			bulletSprite.Y = AbsoluteY;
			
			Parent.GameRef.Display.Renderer.Render(bulletSprite, 10);
		}
		
		public override void Update()
		{
			
			this.X += -Math.Sin(angle*Math.PI/180)*magnitude;
			this.Y += Math.Cos(angle*Math.PI/180)*magnitude;
			bulletSprite.Update(1);
		}
		
		public override void CollideWith(Component obj, Edge edge, double collX1, double collY1, double collX2, double collY2)
		{
			
		}
	}
}
