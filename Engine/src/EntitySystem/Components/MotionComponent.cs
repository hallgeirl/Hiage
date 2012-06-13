using System;

namespace Engine
{
	public class MotionComponent : GOComponent
	{
		Vector velocity = new Vector();
		Vector accelleration = new Vector();
		
		public MotionComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
			OwnerSet += delegate {
				Owner.BroadcastMessage(new VelocityChangedMessage(Velocity));
				Owner.BroadcastMessage(new AccellerationChangedMessage(Accelleration));
				Owner.ComponentAdded += delegate(object sender, GOComponent component) {
					component.SendMessage(new VelocityChangedMessage(Velocity));
					component.SendMessage(new AccellerationChangedMessage(Accelleration));
				};
			};
		}
	
		public Vector Velocity
		{
			get { return velocity; } 
		}
		
		public Vector Accelleration
		{
			get { return accelleration; }
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
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "motion")
				throw new LoggedException("Cannot load MotionComponent from descriptor " + descriptor.Name);
		
			foreach (ComponentDescriptor d in descriptor.Subcomponents)
			{
				if (d.Name == "velocity")
					Velocity.Set(double.Parse(d["x"]), double.Parse(d["y"]));
				else if (d.Name == "accelleration")
					Accelleration.Set(double.Parse(d["x"]), double.Parse(d["y"]));
			}
		}
	}
}

