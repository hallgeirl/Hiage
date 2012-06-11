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
			
			Velocity.PropertyChanged += delegate {
				Owner.BroadcastMessage(new VelocityChangedMessage(Velocity));
			};
			
			OwnerSet += delegate {
				Owner.BroadcastMessage(new VelocityChangedMessage(Velocity));
			};
			
			if (Owner != null)
				Owner.BroadcastMessage(new VelocityChangedMessage(Velocity));
		}
		
		public Vector Velocity
		{
			get;
			private set;
		}
		
		public Vector Accelleration
		{
			get;
			private set;
		}
		
		public override string Family 
		{
			get 
			{
				return "motion";
			}
		}
		
		public void Update(double frameTime, int axis)
		{
			Velocity[axis] += Accelleration[axis]*frameTime;
			Accelleration[axis] = 0;
			
			FrictionComponent friction = (FrictionComponent)Owner.GetComponent("friction");
			
			if (friction != null)
				friction.ClampVelocity(Velocity, frameTime);
		}
		
		public override void Update (double frameTime)
		{
			Velocity.Add(Accelleration*frameTime);
			Accelleration.Set(0, 0);
			
			FrictionComponent friction = (FrictionComponent)Owner.GetComponent("friction");
			
			if (friction != null)
				friction.ClampVelocity(Velocity, frameTime);
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
	}
}

