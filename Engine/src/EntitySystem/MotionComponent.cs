using System;

namespace Engine
{
	public class MotionComponent : GOComponent
	{
		public MotionComponent() : base()
		{
			Velocity = new Vector();
			Accelleration = new Vector();
		}
		
		public MotionComponent(Vector vel, Vector accel) : base()
		{
			Velocity = vel.Copy();
			Accelleration = accel.Copy();
		}
		
		public Vector Velocity
		{
			get;
			protected set;
		}
		
		public Vector Accelleration
		{
			get;
			protected set;
		}
		
		public override string Family 
		{
			get 
			{
				return "motion";
			}
		}
		
		public override void Update (double frameTime)
		{
			Velocity += Accelleration*frameTime;
			Accelleration.Set(0, 0);
			
			FrictionComponent friction = (FrictionComponent)Owner.GetComponent("friction");
			
			if (friction != null)
				friction.ClampVelocity(Velocity, frameTime);
		}
	}
}

