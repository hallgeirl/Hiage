//using System;
//using System.Collections.Generic;
//using Engine;
//
//namespace Mario
//{
//	public class Coin : GameObjectComponent
//	{
//		public Coin (Game game, Dictionary<string, BoundingPolygon> polygons) 
//			: base(game, polygons)
//		{
//			//CurrentSprite.PlayAnimation("normal", true);
//		}
//		
//		protected override void SetupStates ()
//		{
//		}
//		
//		
//		public override BoundingPolygon BoundingPolygon 
//		{
//			get 
//			{
//				return boundingPolygons["normal"];
//			}
//		}
//		
//		
//		public override void UpAction ()
//		{
//		}
//		
//		
//		public override void LeftAction ()
//		{
//		}
//		
//		
//		public override void DownAction ()
//		{
//		}
//		
//		
//		public override void RightAction ()
//		{
//		}		
//	}
//}
//
