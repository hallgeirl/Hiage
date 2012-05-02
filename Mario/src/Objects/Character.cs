
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
		protected int standState, walkState, runState, inAirState, dieState;
		protected Timer dieTimer = new Timer();

		
		//Just pass on the constructor stuff to base
		public Character(Vector position, Vector velocity, Sprite sprite, Renderer renderer, IController controller, 
		                 WorldPhysics worldPhysics, ObjectPhysics objectPhysics, Dictionary<string, BoundingPolygon> boundingPolygons,
		                 double runSpeed, double maxSpeed) 
			: base(position, velocity, sprite, renderer, controller, worldPhysics, objectPhysics, boundingPolygons) 
		{
			Sprite.PlayAnimation("stand", true);
			MaxSpeed = maxSpeed;
			RunSpeed = runSpeed;
		}
		
		protected override void SetupStates ()
		{
			standState = AddState(delegate { 
				if (!OnGround)
				{
					currentState = inAirState;
				}
				else
				{
					if (Math.Abs(Velocity.X) > 1e-10)
						currentState = walkState;
				}
				
				animationSpeedFactor = 1;
				Sprite.PlayAnimation("stand", false); 
			});
			
			walkState = AddState(delegate {
				if (!OnGround)
				{
					currentState = inAirState;
				}
				else
				{
					if (Math.Abs(Velocity.X) < 1e-10)
						currentState = standState;
					else if (Math.Abs(Velocity.X) > RunSpeed)
						currentState = runState;
				}
				Sprite.PlayAnimation("walk", false);
			});
			
			runState = AddState(delegate {
				if (!OnGround)
					currentState = inAirState;
				else
				{
					if (Math.Abs(Velocity.X) < RunSpeed)
						currentState = walkState;
				}
				Sprite.PlayAnimation("run", false);
			});
			
			inAirState = AddState(delegate {
				if (OnGround)
					currentState = standState;
				else
				{
					if (Velocity.Y > 0)
						Sprite.PlayAnimation("jump", false);
					else
						Sprite.PlayAnimation("fall", false);
				}
			});
			
			currentState = standState;
		}
		
		public override void UpdateVelocity(double frameTime)
		{
			base.UpdateVelocity(frameTime);
			
			//Limit speed
			if (Velocity.X < -MaxSpeed)
				Velocity.X = -MaxSpeed;
			else if (Velocity.X > MaxSpeed)
			    Velocity.X = MaxSpeed;
		}
		
		public override void Update(double frameTime)
		{
			base.Update(frameTime);
			if (Velocity.X > 1e-12)
				Sprite.Flipped = false;
			else if (Velocity.X < -1e-12)
			    Sprite.Flipped = true;
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
			currentState = dieState;
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
