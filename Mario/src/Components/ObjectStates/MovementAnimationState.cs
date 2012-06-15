using System;
using Engine;

namespace Mario
{
	public abstract class MovementAnimationState : ObjectState
	{
		protected int framesOnGround = 0;
		
		public MovementAnimationState (StateMachineComponent owner) : base(owner)
		{
			StateActivated += delegate {
				if (Owner.Owner != null)
				if (Owner.Owner.ObjectName == "mario")
					Console.WriteLine("State set to " + Name);
			};
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is CollisionEventMessage)
			{
				CollisionResult result = ((CollisionEventMessage)message).Result;
				if (result.hasIntersected && result.hitNormal.Y > 0.1)
					framesOnGround = 0;
			}
			else if (message is VelocityChangedMessage)
				Velocity = ((VelocityChangedMessage)message).Velocity;
			else if (message is RenderableChangedMessage)
				Renderable = ((RenderableChangedMessage)message).Renderable;
		}
		
		protected Vector Velocity
		{
			get; 
			private set;
		}
		
		private IRenderable Renderable
		{
			get;set;
		}
		
		public override void Update(double frameTime)
		{
			if (Renderable == null) 
				return;
			
			if (Velocity.X < -1e-10)
				Renderable.Flipped = true;
			else if (Velocity.X > 1e-10)
				Renderable.Flipped = false;	
				
			Renderable.AnimationSpeedFactor = Math.Abs(Velocity.X)*5.0/400.0+0.1;
			
		}
	}
}

