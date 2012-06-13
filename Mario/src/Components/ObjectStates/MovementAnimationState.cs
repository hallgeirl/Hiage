using System;
using Engine;

namespace Mario
{
	public abstract class MovementAnimationState : ObjectState
	{
		protected int framesOnGround = 0;
		
		public MovementAnimationState (StateMachineComponent owner) : base(owner)
		{
		}
		
		public override void ReceiveMessage (Message message)
		{
			
			if (message is CollisionEventMessage)
			{
				CollisionResult result = ((CollisionEventMessage)message).Result;
				if (result.hasIntersected && result.hitNormal.Y > 0.1)
					framesOnGround = 0;
			}
			
//			if (message is CollidedWithGroundMessage)
//				framesOnGround = 0;
//			
			if (message is VelocityChangedMessage)
				Velocity = ((VelocityChangedMessage)message).Velocity;
		}
		
		protected Vector Velocity
		{
			get; 
			private set;
		}
		
		public override void Update(double frameTime)
		{
			if (Velocity.X < -1e-10)
				Owner.Owner.BroadcastMessage(new SetHorizontalFlipMessage(true));
			else if (Velocity.X > 1e-10)
				Owner.Owner.BroadcastMessage(new SetHorizontalFlipMessage(false));
				
			Owner.Owner.BroadcastMessage(new SetAnimationSpeedFactorMessage(Math.Abs(Velocity.X)*5/400+0.1));
		}
	}
}

