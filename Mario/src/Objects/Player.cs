
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
		protected int crouchState, growState, shrinkState;
		Timer growTimer = new Timer();
		Timer invincibleTimer = new Timer();
		
		public Player (Game game, //GameObject attributes
		               Dictionary<string, BoundingPolygon> boundingPolygons, //PhysicalObject attributes
		               double runSpeed, double maxSpeed, 	//Character attributes
		               PlayerState state)
			: base(game, boundingPolygons, runSpeed, maxSpeed) 
		{
			PlayerState = state;
			
			invincibleTimer.Start();			
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
				if (framesInCurrentState == 0)
				{
					game.SimulationSpeed = 0;
					growTimer.Restart();
					growTimer.Start();
				}
				
				if (growTimer.Elapsed >= 1000)
				{
					//CurrentSprite = Sprites["big"];
					game.SimulationSpeed = Game.DefaultSimulationSpeed;
					PlayerState.HealthStatus = PlayerState.Health.Big;
					
					SetState(standState);
					growTimer.Stop();
				}
			});			
			
			shrinkState = AddState(delegate {
				if (framesInCurrentState == 0)
				{
					game.SimulationSpeed = 0;
					growTimer.Restart();
					growTimer.Start();
				}
					
				if (growTimer.Elapsed >= 1000)
				{
					//CurrentSprite = Sprites["small"];
					game.SimulationSpeed = Game.DefaultSimulationSpeed;
					PlayerState.HealthStatus = PlayerState.Health.Small;
					
					SetState(standState);
					growTimer.Stop();
					invincibleTimer.Restart();
					invincibleTimer.Start();
				}
			});			
			
			dieState = AddState(delegate {
				if (framesInCurrentState == 0)
				{
					dieTimer.Restart();
					dieTimer.Start();
					//Controller = new MarioDiesController();
					CanCollide = false;
				}
				if (dieTimer.Elapsed > 1000 && dieTimer.Elapsed < 1100)
				{
					game.SimulationSpeed = Game.DefaultSimulationSpeed;
					Velocity.Y = 300;
				}
				else if (dieTimer.Elapsed < 1000)
				{
					game.SimulationSpeed = 0;
				}
				CurrentSprite.PlayAnimation("die", false);
			});
		}
		
		public override BoundingPolygon BoundingPolygon
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
			base.Update(frameTime);
			if (currentState == dieState)
			{
			}
			else
			{
				if (crouching)
				{
					crouching = false;
					Position.Y += 2;
				}
			
				animationSpeedFactor = (Math.Abs(Velocity.X)*5 / MaxSpeed)+0.1;
			}
		}

		
		public override void Collide (ICollidable o, Vector edgeNormal, CollisionResult collisionResult)
		{
			if (currentState == dieState) return;
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
			else if (o is Mushroom && ((Mushroom)o).MushroomType == Mushroom.ItemType.RedMushroom)
			{
				Grow();
				((Mushroom)o).Delete = true;
			}
			else if (o is BasicGroundEnemy)
			{
				BasicGroundEnemy e = (BasicGroundEnemy)o;
				if (BoundingPolygon.Bottom >= o.BoundingPolygon.Top && e.Stompable && !e.Dying)
				{
					if (collisionResult.hasIntersected)
					{
						Velocity.Y = 200;
						e.Kill();
						game.Audio.PlaySound("stomp");
					}
				}
				else if (!e.Dying)
				{
					Shrink();
				}
			}
			
			
		}
		
		public override void Collide (BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
			if (currentState == dieState) return;
			base.Collide (p, collisionNormal, collisionResult);
			if (collisionResult.hitNormal.X > 0.8 && sliding)
			{
				sliding = false;
			}
		}
		
		public override void UpAction()
		{
			if (currentState == dieState) return;
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
			if (currentState == dieState) return;
			crouching = true;
			//Position.Y -= 2;
			
			if (currentState != crouchState)
			{
				SetState(crouchState);
				
			}
		}
		
		public override void LeftAction()
		{
			if (currentState == dieState) return;
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
			if (currentState == dieState) return;
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
		
		protected void Shrink()
		{
			if (invincibleTimer.Elapsed < 1000) return;
			if (currentState != dieState && PlayerState.HealthStatus == PlayerState.Health.Small)
			{
				game.Audio.PlaySound("mario-death");
				game.Audio.PlayMusic(null, null);
				Kill ();
				SetState(dieState);
			}
			else if (currentState != shrinkState && PlayerState.HealthStatus == PlayerState.Health.Big)
			{
				SetState (shrinkState);
			}
		}
	}
}
