using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	public class Coin : GameObject
	{
		public Coin (Game game, Vector position, Dictionary<string, Sprite> sprites, string defaultSprite, Dictionary<string, BoundingPolygon> polygons) 
			: base(game, position, new Vector(0,0), sprites, defaultSprite, null, polygons)
		{
			CurrentSprite.PlayAnimation("normal", true);
		}
		
		protected override void SetupStates ()
		{
		}
		
		
		public override BoundingPolygon BoundingBox 
		{
			get 
			{
				return boundingPolygons["normal"];
			}
		}
		
		
		public override void UpAction ()
		{
		}
		
		
		public override void LeftAction ()
		{
		}
		
		
		public override void DownAction ()
		{
		}
		
		
		public override void RightAction ()
		{
		}		
	}
}

