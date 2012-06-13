using System;

namespace Engine
{
	public class GravityComponent : GOComponent
	{
		double g;
	
		public GravityComponent(ComponentDescriptor descriptor, ResourceManager resources, double gravity) : base(descriptor, resources)
		{
			g = gravity;
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
			Accelleration.Y -= g;
		}
		
		public override void ReceiveMessage (Message message)
		{		
			if (message is AccellerationChangedMessage)
			{
				Accelleration = ((AccellerationChangedMessage)message).Accelleration;
			}
		}
		
		private Vector Accelleration
		{
			get;
			set;
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "gravity")
				throw new LoggedException("Cannot load GravityComponent from descriptor " + descriptor.Name);
		}
	}
}

