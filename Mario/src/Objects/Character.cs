
using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	/// <summary>
	/// Represents characters (monsters/players), and handles animations played for these.
	/// </summary>
	public abstract class Character : PhysicalObject
	{
		protected int standState, walkState, runState, inAirState, dieState, brakeState;
		protected Timer dieTimer = new Timer();

		
		//Just pass on the constructor stuff to base
		public Character(Game game,  
		                  Dictionary<string, BoundingPolygon> boundingPolygons,
		                 double runSpeed, double maxSpeed) 
			: base(game, boundingPolygons) 
		{
			//CurrentSprite.PlayAnimation("stand", true);
			MaxSpeed = maxSpeed;
			RunSpeed = runSpeed;
		}
		
		
		protected override void SetupStates ()
		{
			standState = AddState(delegate { 
				if (!OnGround)
				{
					SetState(inAirState);
				}
				else
				{
					if (Math.Abs(Velocity.X) > 1e-10)
						SetState(walkState);
				}
				
				animationSpeedFactor = 1;
				CurrentSprite.PlayAnimation("stand", false); 
			});
			
			walkState = AddState(delegate {
				if (!OnGround)
				{
					SetState(inAirState);
				}
				else
				{
					if (Math.Abs(Velocity.X) < 1e-10)
						SetState(standState);
					else if (Math.Abs(Velocity.X) > RunSpeed)
						SetState(runState);
				}
				CurrentSprite.PlayAnimation("walk", false);
			});
			
			runState = AddState(delegate {
				if (!OnGround)
					SetState(inAirState);
				else
				{
					//if (Velocity.DotProduct(prevAccelVector) < 0 && Math.Abs(prevAccelVector.X) > friction*2)
					//	SetState(brakeState);
					if (Math.Abs(Velocity.X) < RunSpeed)
						SetState(walkState);
				}
				CurrentSprite.PlayAnimation("run", false);
			});
			
			inAirState = AddState(delegate {
				if (OnGround)
					SetState(standState);
				else
				{
					if (Velocity.Y > 0)
						CurrentSprite.PlayAnimation("jump", false);
					else
						CurrentSprite.PlayAnimation("fall", false);
				}
			});
			
			brakeState = AddState(delegate {
				if (!OnGround)
					SetState(inAirState);
				else
				{
					//if (Velocity.X * prevAccelVector.X >= 0 || (Math.Abs(prevAccelVector.X) <= friction*2 && framesInCurrentState >= 2) )
						SetState(standState);
				}
				CurrentSprite.PlayAnimation("brake", false);
			});
			
			SetState(standState);
			//currentState = standState;
		}
		
		/*public override void UpdateVelocity(double frameTime)
		{
			base.UpdateVelocity(frameTime);
			
			//Limit speed
			if (Velocity.X < -MaxSpeed)
				Velocity.X = -MaxSpeed;
			else if (Velocity.X > MaxSpeed)
			    Velocity.X = MaxSpeed;
		}*/
		
		public override void Update(double frameTime)
		{
			base.Update(frameTime);
			if (Velocity.X > 1e-12)
				CurrentSprite.Flipped = false;
			else if (Velocity.X < -1e-12)
			    CurrentSprite.Flipped = true;
		}
		
		public double RunSpeed
		{
			get;
			private set;
		}
		
		public double MaxSpeed
		{
			get;
			private set;
		}
		
		public void Kill()
		{
			SetState(dieState);
			dieTimer.Start();
			
			Dying = true;
		}
		
		public bool Dying
		{
			get;
			private set;
		}
	}
}
