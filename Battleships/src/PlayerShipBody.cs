using Engine;
using System;

namespace Battleships
{
	
	/// <summary>
	/// Body of the player's ship
	/// </summary>
	public class PlayerShipBody : Component
	{
		Sprite bodySprite;
		
		public PlayerShipBody(Ship parent, double x, double y, Arena arena) : base(parent, x, y, false, arena, 32, 48)
		{
			bodySprite = new Sprite(parent.GameRef.Resources.GetSpriteDescriptor("playership"), parent.GameRef.Resources);
		}
		
		public override void Fire()
		{
		}
		
		public override void Destroy()
		{
		}
		
		public override void Render()
		{
			bodySprite.X = AbsoluteX;
			bodySprite.Y = AbsoluteY;
			bodySprite.Rotation = Parent.Rotation;
			
			Parent.GameRef.Display.Renderer.Render(bodySprite);
			
/*			Parent.GameRef.Display.Renderer.DrawLine(Left, 0, Left, 1000);
			Parent.GameRef.Display.Renderer.DrawLine(Right, 0, Right, 1000);
			Parent.GameRef.Display.Renderer.DrawLine(0, Top, 1000, Top);
			Parent.GameRef.Display.Renderer.DrawLine(0, Bottom, 1000, Bottom);*/

		}
		
		public override void Update()
		{
			bodySprite.Update(1);
		}
		
		public override void CollideWith(Component obj, Edge edge, double collX1, double collY1, double collX2, double collY2)
		{
			if (obj is SimpleBullet)
			{
				//Console.WriteLine("Coll X:" + collX2 + " X:" + obj.AbsoluteX);
				ArenaRef.MoveObject(obj, obj.AbsoluteX, obj.AbsoluteY, collX2, collY2, obj.Width, obj.Height, obj.Width, obj.Height);
				obj.X = collX2;
				obj.Y = collY2;
				//Console.WriteLine("New X:" + obj.AbsoluteX);
				obj.Destroy();
				
			}
			if (obj is PlayerShipBody)
			{
				Parent.Velocity *= -1;
				Parent.X = OldX;
				Parent.Y = OldY;
			}
		}
	}
	
}
