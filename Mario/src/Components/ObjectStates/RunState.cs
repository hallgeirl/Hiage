using System;
using Engine;

namespace Mario
{
	public class RunState : MovementAnimationState
	{
		private double runSpeed;
		
		public RunState(StateMachineComponent owner, double runSpeed) : base(owner)
		{
			this.runSpeed = runSpeed;
			StateActivated += delegate {
				if (Owner.Owner != null)
					Owner.Owner.BroadcastMessage(new PlayAnimationMessage("run"));
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
				else if (Math.Abs(Velocity.X) < runSpeed)
					SetState("walk");
			}
			
		}
		
		public override string Name {
			get {
				return "run";
			}
		}
		

//		public override string Family 
//		{
//			get 
//			{
//				return "state_run";
//			}
//		}
	}
}

