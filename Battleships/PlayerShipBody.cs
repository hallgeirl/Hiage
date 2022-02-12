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
		
		public PlayerShipBody(Ship parent, double x, double y) : base(parent, x, y, false, true)
		{
			bodySprite = new Sprite(parent.GameRef.Resources.GetSpriteDescriptor("playership"), parent.GameRef.Resources);
		}
		
		public override void Fire(Vector direction)
		{
		}
		
		public override void Render()
		{
			bodySprite.X = AbsoluteX;
			bodySprite.Y = AbsoluteY;
			bodySprite.Rotation = Parent.Rotation;
			
			Parent.GameRef.Display.Renderer.Render(bodySprite);
		}
		
		public override void Update()
		{
			bodySprite.Update();
		}
	}
	
}
