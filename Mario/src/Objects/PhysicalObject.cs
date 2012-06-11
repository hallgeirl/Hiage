
using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	//Represents all objects which should be affected by physics
	public abstract class PhysicalObject : GameObjectComponent
	{
		private   Timer 		inAirTimer = new Timer();
		protected double 		friction;
		public PhysicalObject (Game game,  
		                       Dictionary<string, BoundingPolygon> boundingPolygons) 
			: base(game, boundingPolygons) 
		{
			inAirTimer.Start();
		}

		#region Event handlers for physical objects
		public delegate void LandedEventHandler();
		public delegate void WallCollisionEventHandler();
		public delegate void FallEventHandler();
		
		public event WallCollisionEventHandler CollidedWithWall;
		public event LandedEventHandler Landed;
		public event FallEventHandler Fall;
		
		protected void OnLanded()
		{
			if (Landed != null)
				Landed();
		}
		
		protected void OnCollidedWithWall()
		{
			if (CollidedWithWall != null)
				CollidedWithWall();
		}
		
		protected void OnFall()
		{
			if (Fall != null)
				Fall();
		}
		
		#endregion
		
		public override void Update(double frameTime)
		{
			base.Update(frameTime);
			
			//Check if we're starting to fall
			if (OnGround && inAirTimer.Elapsed > frameTime*2000)
			{
				OnGround = false;
				OnFall();
			}
		}
		
		#region Properties
		public bool OnGround
		{
			get;
			protected set;
		}
		#endregion
	}
}
