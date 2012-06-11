using System;

namespace Engine
{
	public class GravityComponent : GOComponent
	{
		double g;
		public GravityComponent (double g)
		{
			this.g = g;
		}
		
		public override string Family 
		{
			get 
			{
				return "gravity";
			}
		}
		
		public override void Update (double frameTime)
		{
			//Apply gravity
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			motion.Accelleration.Y -= g;
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}

