
using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	//Represents all objects which should be affected by physics
	public abstract class PhysicalObject : GameObject
	{
		protected WorldPhysics 	worldPhysics;
		protected ObjectPhysics objectPhysics;
		private   Timer 		inAirTimer = new Timer();
		
		public PhysicalObject (Vector position, Vector velocity, Sprite sprite, Renderer renderer, IController controller, 
		                       WorldPhysics worldPhysics, ObjectPhysics objectPhysics, Dictionary<string, BoundingPolygon> boundingPolygons) : base(position, velocity, sprite, renderer, controller, boundingPolygons) 
		{
			this.worldPhysics = worldPhysics;
			this.objectPhysics = objectPhysics;
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
			
			//Apply gravity
			Accellerate(new Vector(0, -worldPhysics.Gravity));
			
			//Pre-calculate the actual friction (based on world- and object physics attributes)
			double friction = objectPhysics.Friction*worldPhysics.GroundFrictionFactor;
			
			if (Velocity.X > friction*frameTime)
				Accellerate(new Vector(-friction, 0));
			else if (Velocity.X < -friction*frameTime)
				Accellerate(new Vector(friction, 0));
			else
				Velocity.X = 0;
			
			//Check if we're starting to fall
			if (OnGround && inAirTimer.Elapsed > frameTime*2000)
			{
				OnGround = false;
				OnFall();
			}
		}
		
		//Handle collisions against edges
		public override void Collide(BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
			base.Collide(p, collisionNormal, collisionResult);
			
			// If intersecting, push back
			/*if (collisionResult.IsIntersecting)
			{
				Position += collisionResult.MinimumTranslationVector;
			}
			else */
			if (collisionResult.WillIntersect)
			{
				remainingFrameTime -= collisionResult.CollisionTime;

				Position += collisionResult.CollisionTime * Velocity * frameTime;

				Velocity = Velocity - ((1.0+objectPhysics.Elasticity)*Velocity.DotProduct(collisionNormal))*collisionNormal;

   				/*
				 * Run the event handlers
				 */
				if (Math.Abs(collisionNormal.X) > 0.8)
					OnCollidedWithWall();
				
				if (collisionNormal.Y > 0.5)
				{
					if (!OnGround)
					{
						OnLanded();
						OnGround = true;
					}
					inAirTimer.Restart();
				}
			}
		}
		
		#region Properties
		public bool OnGround
		{
			get;
			private set;
		}
		#endregion
	}
}
