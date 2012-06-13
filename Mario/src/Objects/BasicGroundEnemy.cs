//
//using System;
//using System.Collections.Generic;
//using Engine;
//
//namespace Mario
//{
//	public class BasicGroundEnemy : Character
//	{
//		public BasicGroundEnemy (Game game, //GameObject attributes
//		               Dictionary<string, BoundingPolygon> boundingPolygons,	//PhysicalObject attributes
//		               double runSpeed, double maxSpeed) 	//Character attributes
//			: base(game, boundingPolygons, runSpeed, maxSpeed) 
//		{
//			Stompable = true;
//		}
//		
//		protected override void SetupStates ()
//		{
//			base.SetupStates();
//			
//			dieState = AddState(delegate {
//				CurrentSprite.PlayAnimation("die", false);
//			});
//		}
//		
//		public override void Update(double frameTime)
//		{
//			base.Update(frameTime);
//			
//			if (dieTimer.Elapsed > 1000)
//				Delete = true;
//		}
//
//		public override void UpAction ()
//		{
//		}
//
//		public override void DownAction()
//		{
//		}
//
//		public override void RightAction()
//		{
//			if (currentState != dieState)
//				Accellerate(new Vector(300, 0));
//		}
//		
//		public override void LeftAction()
//		{
//			if (currentState != dieState)
//				Accellerate(new Vector(-300, 0));
//		}
//		
//		public override BoundingPolygon BoundingPolygon 
//		{
//			get 
//			{
//				var poly = boundingPolygons["normal"];
//				return poly;
//			}
//		}
//		
//		public bool Stompable
//		{
//			get;
//			private set;
//		}
//		
//
//	}
//}
