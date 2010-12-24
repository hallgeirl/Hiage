
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
		                       WorldPhysics worldPhysics, ObjectPhysics objectPhysics, int width, int height) : base(position, velocity, sprite, renderer, controller, width, height) 
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
			if (collisionResult.WillIntersect && (Right <= p.Left || Left >= p.Right) && p.Vertices.Count == 2 && Math.Abs(p.Vertices[0].Y - p.Vertices[1].Y) < Constants.MinDouble)
			{
				Log.Write("FOO");
				return;
			}
			//if (collisionNormal.DotProduct(Velocity) > Constants.MinDouble) return;
			// Ignore "collisions" where the only edge faces in the movement direction.
			//if (p.Vertices.Count == 2 && p.EdgeNormals[0].DotProduct(Velocity) > Constants.MinDouble) return;
			
			base.Collide(p, collisionNormal, collisionResult);
			
			// If intersecting, push back
			/*if (collisionResult.IsIntersecting)
			{
				Position += collisionResult.MinimumTranslationVector;
			}
			else */if (collisionResult.WillIntersect)
			{
				remainingFrameTime -= collisionResult.CollisionTime;

				Position += collisionResult.CollisionTime * Velocity * frameTime;

				Velocity = Velocity - ((1.0+objectPhysics.Elasticity)*Velocity.DotProduct(collisionNormal))*collisionNormal;

   				/*
				 * Run the event handlers
				 */
				if (Math.Abs(collisionNormal.X) > 0.8)
					OnCollidedWithWall();
				
				if (collisionNormal.Y > 0.8)
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
		
		/*public override int Width
		{
			get;
			protected set;
		}
		
		public override int Height 
		{
			get;
			protected set;
		}*/

	}
}
