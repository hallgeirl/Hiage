using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	public class Mushroom : PhysicalObject
	{
		public enum ItemType
		{
			RedMushroom,
			GreenMushroom
		}
		
		public Mushroom(Game game, 
		                    Vector position, 
		                    Dictionary<string, Sprite> sprites, string defaultSprite, 
		                    WorldPhysics worldPhysics, ObjectPhysics objectPhysics, 
		                    Dictionary<string, BoundingPolygon> polygons, 
		                    ItemType itemType) 
			: base(game, position, new Vector(0,0), sprites, defaultSprite, new DumbGroundAI(), worldPhysics, objectPhysics, polygons)
		{
			CurrentSprite.PlayAnimation(Sprite.DEFAULT_ANIMATION, true);
			MushroomType = itemType;
		}
		
		public ItemType MushroomType
		{
			get; private set;
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
			Accellerate(new Vector(-100, 0));
		}
		
		
		public override void DownAction ()
		{
		}
		
		
		public override void RightAction ()
		{
			Accellerate(new Vector(100, 0));
		}		
	}
}

