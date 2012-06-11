using System;

namespace Engine
{
	public class VelocityChangedMessage : Message
	{
		public VelocityChangedMessage (Vector velocity)
		{
			Velocity = velocity;
		}
		
		public Vector Velocity
		{
			get;
			private set;
		}
	}
}

