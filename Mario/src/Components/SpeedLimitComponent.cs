using System;
using Engine;

namespace Mario
{
	public class SpeedLimitComponent : GOComponent
	{
		public SpeedLimitComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
		}
		
		public override string Family 
		{
			get 
			{
				return "speedlimit";
			}
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is VelocityChangedMessage)
			{
				Velocity = ((VelocityChangedMessage)message).Velocity;
			}
		}
		
		public override void Update (double frameTime)
		{
			if (Velocity == null) return;
			
			if (Velocity.X < -SpeedLimit)
				Velocity.X = -SpeedLimit;
			else if (Velocity.X > SpeedLimit)
				Velocity.X = SpeedLimit;
		}
		
		private double SpeedLimit
		{
			get;
			set;
		}
		
		private Vector Velocity
		{
			get; set;
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "speedlimit")
				throw new LoggedException("Cannot load SpeedLimitComponent from descriptor " + descriptor.Name);
			
			SpeedLimit = double.Parse(descriptor["maxspeed"]);
		}
	}
}

