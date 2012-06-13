using System;
using Engine;

namespace Mario
{
	public class InAirState : MovementAnimationState
	{
		//private int framesInAir = 0;
		bool falling = false;
		public InAirState (StateMachineComponent owner) : base(owner)
		{
			StateActivated += delegate {
				if (Owner.Owner != null)
				{
					if (Velocity.Y < 0)
					{
						Owner.Owner.BroadcastMessage(new PlayAnimationMessage("fall"));
						falling = true;
					}
					else
					{
						Owner.Owner.BroadcastMessage(new PlayAnimationMessage("jump"));
						falling = false;
					}
				}
			};
		}
		
		public override void Update (double frameTime)
		{
			base.Update(frameTime);
			if (Velocity.Y < 0 && !falling)
			{
				Owner.Owner.BroadcastMessage(new PlayAnimationMessage("fall"));
				falling = true;
			}
			else if (Velocity.Y > 0 && falling)
			{
				Owner.Owner.BroadcastMessage(new PlayAnimationMessage("jump"));
				falling = false;
			}
		}
		
		public override string Name {
			get {
				return "inair";
			}
		}
		
		public override void ReceiveMessage (Message message)
		{
			base.ReceiveMessage(message);
			
			if (message is CollisionEventMessage)
			{
				CollisionResult result = ((CollisionEventMessage)message).Result;
				if (result.hasIntersected && result.hitNormal.Y > 0.1)
				{
					Owner.Owner.BroadcastMessage(new LandedMessage());
					SetState ("stand");
				}
			}
//			if (message is CollidedWithGroundMessage)
//			{
//				Owner.Owner.BroadcastMessage(new LandedMessage());
//			}
		}
		
//		public override string Family 
//		{
//			get 
//			{
//				return "state_inair";
//			}
//		}
	}
}

