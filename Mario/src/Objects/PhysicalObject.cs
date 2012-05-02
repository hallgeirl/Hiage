
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
		
		public PhysicalObject (Game game, Vector position, Vector velocity, Sprite sprite, Renderer renderer, IController controller, 
		                       WorldPhysics worldPhysics, ObjectPhysics objectPhysics, Dictionary<string, BoundingPolygon> boundingPolygons) : base(game, position, velocity, sprite, renderer, controller, boundingPolygons) 
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
		
		public override void UpdateAccelleration(double frameTime)
		{
			base.UpdateAccelleration(frameTime);
			
			//Apply gravity
			Accellerate(new Vector(0, -worldPhysics.Gravity));
			
			//Pre-calculate the actual friction (based on world- and object physics attributes)
			double friction = objectPhysics.Friction*worldPhysics.GroundFrictionFactor;
			
			if (Velocity.X > friction*frameTime)
				Accellerate(new Vector(-friction, 0));
			else if (Velocity.X < -friction*frameTime)
				Accellerate(new Vector(friction, 0));
		}
		
		public override void UpdateVelocity(double frameTime)
		{
			base.UpdateVelocity(frameTime);
			
			//Pre-calculate the actual friction (based on world- and object physics attributes)
			double friction = objectPhysics.Friction*worldPhysics.GroundFrictionFactor;
			
			if (Velocity.X <= friction*frameTime && Velocity.X >= -friction*frameTime)
				Velocity.X = 0;
		}
		
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
		
		//Handle collisions against edges
		public override void Collide(BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
			//if (collisionResult.isIntersecting) return;
			
			base.Collide(p, collisionNormal, collisionResult);
			
			// If intersecting, push back
			if (collisionResult.isIntersecting)
			{
				Position += collisionResult.minimumTranslationVector;
			}
			else 
			{
				remainingFrameTime -= collisionResult.collisionTime;

				if (collisionResult.hasIntersected)
					Position -= (1-collisionResult.collisionTime + 1e-6) * Velocity * frameTime;
				else
					Position -= collisionResult.minimumTranslationVector;

				Velocity = Velocity - ((1.0+objectPhysics.Elasticity)*Velocity.DotProduct(collisionNormal))*collisionNormal;
				Position += remainingFrameTime * Velocity * frameTime;

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
		//	}
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
