
using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	/// <summary>
	/// Represents the player (Mario (or possibly in the future, Luigi)).
	/// </summary>
	public class Player : Character
	{
		
		public enum HealthStatus
		{
			Big,
			Flower,
			Small
		}
		
		//HealthStatus health;
		bool crouching = false;
		bool sliding = false;
		double oldFriction;
		protected int crouchState;
		
		
		public Player (Vector position, Vector velocity, Sprite sprite, Renderer renderer, IController controller, //GameObject attributes
		               WorldPhysics worldPhysics, ObjectPhysics objectPhysics, Dictionary<string, BoundingPolygon> boundingPolygons, //PhysicalObject attributes
		               double runSpeed, double maxSpeed, 	//Character attributes
		               PlayerState state)
			: base(position, velocity, sprite, renderer, controller, worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed) 
		{
			//health = Player.HealthStatus.Small;
			oldFriction = objectPhysics.Friction;
			PlayerState = state;
		}
		
		protected override void SetupStates ()
		{
			base.SetupStates();		
			
			crouchState = AddState(delegate {
				if (!crouching)
					currentState = standState;
				
				Sprite.PlayAnimation("crouch", false);
			});
		}
		
		public override BoundingPolygon BoundingBox
		{
			get
			{ 
				if (boundingPolygons == null) return null;
				else return boundingPolygons["small-standing"];
			}
		}
		
		public override void Update(double frameTime)
		{
			if (crouching)
			{
				crouching = false;
				Position.Y += 2;
			}
			
			
			base.Update(frameTime);
			animationSpeedFactor = (Math.Abs(Velocity.X)*5 / MaxSpeed)+0.1;
			
			
			#region Old code
			//Update animations
			/*if (crouching)
			{
				Sprite.PlayAnimation("crouch", false);
			}
			else
			{
				if (OnGround)
				{
					//If we're standing still on the ground, play the stand animation
					if (Velocity.X > -1e-12 && Velocity.X < 1e-12)
						Sprite.PlayAnimation("stand", false);
					else
					{
						if ((accelVector.X > -objectPhysics.Friction*1.1 && Velocity.X > 0) || (accelVector.X < objectPhysics.Friction*1.1 && Velocity.X < 0))
						{
							//If we have an acceleration vector pointing in the same direction as the velocity (excluding the friction acceleration), play walk or run animation
							if (Math.Abs(Velocity.X) > RunSpeed && Sprite.HasAnimation("run"))
								Sprite.PlayAnimation("run", false);
							else
								Sprite.PlayAnimation("walk", false);
						}
						else
						{
							//Else, play the brake animation if it exists
							if (Sprite.HasAnimation("brake"))
								Sprite.PlayAnimation("brake", false);
							else
								Sprite.PlayAnimation("walk", false);
						}
					}
				}
				else
				{
					//If we're in the air, we're either jumping or falling
					if (Velocity.Y > 0)
						Sprite.PlayAnimation("jump", false);
					else
						Sprite.PlayAnimation("fall", false);
				}
			}*/
			#endregion
			
		}

		
		public override void Collide (ICollidable o, Vector edgeNormal, CollisionResult collisionResult)
		{
			if (o is Coin)
			{
				PlayerState.Coins++;
				if (PlayerState.Coins >= 100)
				{
					PlayerState.Lives += PlayerState.Coins / 100;
					PlayerState.Coins = PlayerState.Coins % 100;
				}
				((Coin)o).Delete = true;
			}
			
			if (BoundingBox.Bottom >= o.BoundingBox.Top && o is BasicGroundEnemy && ((BasicGroundEnemy)o).Stompable && !((BasicGroundEnemy)o).Dying)
			{
				if (collisionResult.hasIntersected)
				{
					BasicGroundEnemy enemy = (BasicGroundEnemy)o;
					Velocity.Y = 200;
					enemy.Kill();
				}
			}
		}
		
		public override void Collide (BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
			base.Collide (p, collisionNormal, collisionResult);
			if (collisionResult.hitNormal.X > 0.8 && sliding)
			{
				sliding = false;
				objectPhysics.Friction = oldFriction;
			}
		}
		
		public void Slide()
		{
			sliding = true;
			objectPhysics.Friction = 0;
		}
				
		public override void UpAction()
		{
			if (OnGround)
			{
				Velocity.Y = 200;
				//Accellerate(new Vector(0,200));
			}
		}
		
		public override void DownAction()
		{
			crouching = true;
			Position.Y -= 2;
			
			if (currentState != crouchState)
			{
				currentState = crouchState;
				
			}
		}
		
		public override void LeftAction()
		{
			if (OnGround)
			{
				if (!crouching)
					Accellerate(new Vector(-400, 0));
			}
			else
				Accellerate(new Vector(-200, 0));
		}
		
		public override void RightAction()
		{
			if (OnGround)
			{
				if (!crouching)
					Accellerate(new Vector(400, 0));
			}
			else 
				Accellerate(new Vector(200, 0));
		}
		
		public PlayerState PlayerState
		{
			get; private set;
		}
	}
}
