
using System;
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
		                       WorldPhysics worldPhysics, ObjectPhysics objectPhysics, int width, int height) : base(position, velocity, sprite, renderer, controller) 
		{
			this.worldPhysics = worldPhysics;
			this.objectPhysics = objectPhysics;
			inAirTimer.Start();
			Width = width;
			Height = height;
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
			
			//Apply physics
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
		public override void Collide(Edge e, CollisionResult collisionResult)
		{
			base.Collide(e, collisionResult);
			if (collisionResult.WillIntersect)
			{
				remainingFrameTime -= collisionResult.CollisionTime;

				Position += collisionResult.MinimumTranslationVector;
				Velocity = Velocity - ((1.0+objectPhysics.Elasticity)*Velocity.DotProduct(e.Normal))*e.Normal;
				
				//Cut off the velocity if it gets too low
				if (Velocity.Length < 1e-10 && Velocity.Length != 0)
				{
					Velocity.X = 0;
					Velocity.Y = 0;
				}
				
				/*
				 * Run the event handlers
				 */
				if (Math.Abs(e.Normal.X) > 0.8)
					OnCollidedWithWall();
				
				//Log.Write("elapsed: " + inAirTimer.Elapsed + ", frametime: " + collisionResult.FrameTime);
				
				if (e.Normal.Y > 0.5)
				{
					if (!OnGround)
					{
						OnLanded();
						OnGround = true;
					}
					inAirTimer.Restart();
				}
				//Log.Write("Velocity after handler: " + Velocity + " Minimum translation: " + collisionResult.MinimumTranslationVector);
			}
		}
		
		#region Properties
		public bool OnGround
		{
			get;
			private set;
		}
		#endregion
		
		public override int Width
		{
			get;
			protected set;
		}
		
		public override int Height 
		{
			get;
			protected set;
		}

	}
}
