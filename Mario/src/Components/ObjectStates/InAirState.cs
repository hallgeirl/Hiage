using System;
using Engine;

namespace Mario
{
	public class InAirState : MovementAnimationState
	{
		//private int framesInAir = 0;
		
		public InAirState (StateMachineComponent owner) : base(owner)
		{
		}
		
		public override void Update (double frameTime)
		{
			if (Velocity.Y < 0)
				Owner.Owner.SendMessage(DrawableComponent.PlayAnimationMessage, "fall");
			else
				Owner.Owner.SendMessage(DrawableComponent.PlayAnimationMessage, "jump");
		}
		
		public override string Name {
			get {
				return "state_inair";
			}
		}
		
		public override void ReceiveMessage (Message message)
		{
			base.ReceiveMessage(message);
			
			if (message is CollidedWithGroundMessage)
			{
				Owner.Owner.BroadcastMessage(new LandedMessage());
				SetState ("state_stand");
			}
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

