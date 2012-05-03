using System;
using System.Collections.Generic;

using Engine;

namespace Mario
{
	public class Icon : GameObject
	{
		public Icon(Game game, Dictionary<string, Sprite> sprites) : base(game, new Vector(), new Vector(0,0), sprites, "icon", null, new Dictionary<string, BoundingPolygon>())
		{
			CurrentSprite.PlayAnimation(Sprite.DEFAULT_ANIMATION, true);
		}
		
		public override void UpAction() {}	
		public override void LeftAction() {}
		public override void DownAction() {}
		public override void RightAction() {}
		
				//Bounding box before update
		public override BoundingPolygon BoundingBox
		{
			get 
			{
				return null;
			}
		}
		
		protected override void SetupStates()
		{
		}
	}
}

