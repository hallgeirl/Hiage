using System;
using Engine;

namespace Mario
{
	public class WalkState : MovementAnimationState
	{
		private double runSpeed;
		
		public WalkState(StateMachineComponent owner, double runSpeed) : base(owner)
		{
			this.runSpeed = runSpeed;
			StateActivated += delegate {
				if (Owner.Owner != null)
					Owner.Owner.BroadcastMessage(new PlayAnimationMessage("walk"));
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
				if (Math.Abs(Velocity.X) < 1e-10)
					SetState("stand");
				else if (Math.Abs(Velocity.X) > runSpeed)
					SetState("run");
			}
		}
		
		
		public override string Name {
			get {
				return "walk";
			}
		}
//		public override string Family 
//		{
//			get 
//			{
//				return "state_walk";
//			}
//		}
	}
}

