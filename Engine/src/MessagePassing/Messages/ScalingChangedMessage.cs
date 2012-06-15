using System;

namespace Engine
{
	public class ScalingChangedMessage : Message
	{
		public ScalingChangedMessage (Vector scaling)
		{
			Scaling = scaling;
		}
		
		public Vector Scaling
		{
			get;
			private set;
		}
	}
}

