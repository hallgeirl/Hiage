using System;

namespace Engine
{
	public class AccellerationChangedMessage : Message
	{
		public AccellerationChangedMessage (Vector accel)
		{
			Accelleration = accel;
		}
		
		public Vector Accelleration
		{
			get;
			private set;
		}
	}
}

