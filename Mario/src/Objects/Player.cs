
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
		protected int crouchState, growState;
		Timer growTimer = new Timer();
		
		public Player (Game game, Vector position, Vector velocity, Dictionary<string, Sprite> sprites, string defaultSprite, IController controller, //GameObject attributes
		               WorldPhysics worldPhysics, ObjectPhysics objectPhysics, Dictionary<string, BoundingPolygon> boundingPolygons, //PhysicalObject attributes
		               double runSpeed, double maxSpeed, 	//Character attributes
		               PlayerState state)
			: base(game, position, velocity, sprites, defaultSprite, controller, worldPhysics, objectPhysics, boundingPolygons, runSpeed, maxSpeed) 
		{
			oldFriction = objectPhysics.Friction;
			PlayerState = state;
			
			
		}
		
		protected override void SetupStates ()
		{
			base.SetupStates();		
			
			crouchState = AddState(delegate {
				if (!crouching)
					SetState(standState);
				
				CurrentSprite.PlayAnimation("crouch", false);
			});
			
			growState = AddState(delegate {
				if (prevState != growState)
				{
					growTimer.Start();
					CurrentSprite = Sprites["big"];
				}
					
				if (growTimer.Elapsed >= 2000)
				{
					PlayerState.HealthStatus = PlayerState.Health.Big;
					//CurrentSprite.PlayAnimation("stand", false);
					
					SetState(standState);
					growTimer.Stop();
				}
				CurrentSprite.PlayAnimation("crouch", false);
			});
		}
		
		public override BoundingPolygon BoundingBox
		{
			get
			{
				if (PlayerState == null)
					return null;
				
				switch (PlayerState.HealthStatus)
				{
				case PlayerState.Health.Small:
					return boundingPolygons["small-standing"];
				case PlayerState.Health.Big:
					return boundingPolygons["big-standing"];
				case PlayerState.Health.Flower:
					return boundingPolygons["big-standing"];
				case PlayerState.Health.Dying:
					return null;
				default:
					throw new ArgumentOutOfRangeException("Invalid value of PlayerState.HealthStatus: " + PlayerState.HealthStatus);
				}
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
				game.Audio.PlaySound("coin");
			}
			
			if (BoundingBox.Bottom >= o.BoundingBox.Top && o is BasicGroundEnemy && ((BasicGroundEnemy)o).Stompable && !((BasicGroundEnemy)o).Dying)
			{
				if (collisionResult.hasIntersected)
				{
					BasicGroundEnemy enemy = (BasicGroundEnemy)o;
					Velocity.Y = 200;
					enemy.Kill();
					game.Audio.PlaySound("stomp");
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
				game.Audio.PlaySound("mario-jump");
				OnGround = false;
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
		
		protected void Grow()
		{
			if (currentState != growState && PlayerState.HealthStatus == PlayerState.Health.Small)
			{
				SetState(growState);
			}
			game.Audio.PlaySound("mario-grow");
		}
	}
}
