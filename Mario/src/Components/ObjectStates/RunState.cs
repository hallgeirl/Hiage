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
		}
		
		public override void Update (double frameTime)
		{
			if (Math.Abs(Velocity.Y) > 50)
			{
				SetState("state_inair");
				Owner.Owner.BroadcastMessage(new InAirMessage());
			}
			else
			{
				if (Math.Abs(Velocity.X) < 1e-10)
					SetState("state_stand");
				else if (Math.Abs(Velocity.X) < runSpeed)
					SetState("state_walk");
			}
			
			Owner.Owner.SendMessage(DrawableComponent.PlayAnimationMessage, "run");
		}
		
		public override string Name {
			get {
				return "state_run";
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

