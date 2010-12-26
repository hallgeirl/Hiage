using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	public class Coin : GameObject
	{
		public Coin (Vector position, Sprite sprite, Renderer renderer, Dictionary<string, BoundingPolygon> polygons) 
			: base(position, new Vector(0,0), sprite, renderer, null, polygons)
		{
			sprite.PlayAnimation("normal", true);
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

