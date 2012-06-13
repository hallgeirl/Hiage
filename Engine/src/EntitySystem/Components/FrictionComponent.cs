using System;

namespace Engine
{
	public class FrictionComponent : GOComponent
	{
		double friction;
		bool applyToX, applyToY;
		
		public FrictionComponent(ComponentDescriptor descriptor, ResourceManager resources, bool applyToX, bool applyToY) : base(descriptor, resources)
		{
			this.applyToX = applyToX;
			this.applyToY = applyToY;
		}
		
//		public FrictionComponent (double friction, bool applyToX, bool applyToY) : base()
//		{
//			this.friction = friction;
//		}
		
		public override string Family {
			get {
				return "friction";
			}
		}
		
		public double Friction
		{
			get { return friction; }
		}
		
		public override void Update (double frameTime)
		{
			if (Velocity == null || Accelleration == null) return;
			
			if (applyToX)
			{
				if (Velocity.X > friction*frameTime)
					Accelleration.X -= friction;
				else if (Velocity.X < -friction*frameTime)
					Accelleration.X += friction;
			}
			else if (applyToY)
			{
				if (Velocity.Y > friction*frameTime)
					Accelleration.Y -= friction;
				else if (Velocity.Y < -friction*frameTime)
					Accelleration.Y += friction;
			}
		}
		
		public void ClampVelocity(Vector velocity, double frameTime)
		{
			if (Velocity == null) return;
			
			if (applyToX)
			{
				if (velocity.X <= Friction*frameTime && velocity.X >= -Friction*frameTime)
				    velocity.X = 0;
			}
			if (applyToY)
			{
				if (velocity.Y <= Friction*frameTime && velocity.Y >= -Friction*frameTime)
				    velocity.Y = 0;
			}
		}
		
		public override void ReceiveMessage (Message message)
		{		
			if (message is VelocityChangedMessage)
				Velocity = ((VelocityChangedMessage)message).Velocity;
			else if (message is AccellerationChangedMessage)
				Accelleration = ((AccellerationChangedMessage)message).Accelleration;
		}		
		
		private Vector Velocity
		{
			get;
			set;
		}
		private Vector Accelleration
		{
			get;
			set;
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "friction")
				throw new LoggedException("Cannot load FrictionComponent from descriptor " + descriptor.Name);
			friction = double.Parse(descriptor["friction"]);
		}
	}
}

