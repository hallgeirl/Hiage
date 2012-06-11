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
		}
		
		public override void Update (double frameTime)
		{
			//MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			if (Math.Abs(Velocity.Y) > 50)
			{
				SetState("state_inair");
				Owner.Owner.BroadcastMessage(new InAirMessage());
			}
			else
			{
				if (Math.Abs(Velocity.X) < 1e-10)
					SetState("state_stand");
				else if (Math.Abs(Velocity.X) > runSpeed)
					SetState("state_run");
			}
			
			Owner.Owner.SendMessage(DrawableComponent.PlayAnimationMessage, "walk");
		}
		
		
		public override string Name {
			get {
				return "state_walk";
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

