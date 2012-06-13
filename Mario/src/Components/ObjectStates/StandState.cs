using System;
using Engine;

namespace Mario
{
	public class StandState : MovementAnimationState
	{
		public StandState(StateMachineComponent owner) : base(owner)
		{
			StateActivated += delegate {
				if (Owner.Owner != null)
					Owner.Owner.BroadcastMessage(new PlayAnimationMessage("stand"));
			};
		}
		
		public override void Update (double frameTime)
		{
			base.Update(frameTime);
			framesOnGround++;
			if (framesOnGround > 3)
			{
				framesOnGround = 0;
				SetState("inair");
				Owner.Owner.BroadcastMessage(new InAirMessage());
			}
			else
			{
				if (Math.Abs(Velocity.X) > 1e-10)
					SetState("walk");
			}
		}
		
		public override string Name {
			get {
				return "stand";
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

