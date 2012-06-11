using System;
using Engine;

namespace Mario
{
	public class StandState : MovementAnimationState
	{
		public StandState(StateMachineComponent owner) : base(owner)
		{
		}
		
		public override void Update (double frameTime)
		{
			framesOnGround++;
			if (framesOnGround > 3)
			{
				framesOnGround = 0;
				SetState("state_inair");
				Owner.Owner.BroadcastMessage(new InAirMessage());
			}
			else
			{
				if (Math.Abs(Velocity.X) > 1e-10)
					SetState("state_walk");
			}
			
			Owner.Owner.SendMessage(DrawableComponent.PlayAnimationMessage, "stand");
		}
		
		public override string Name {
			get {
				return "state_stand";
			}
		}
		
//		public override string Family 
//		{
//			get 
//			{
//				return "state_stand";
//			}
//		}
	}
}

