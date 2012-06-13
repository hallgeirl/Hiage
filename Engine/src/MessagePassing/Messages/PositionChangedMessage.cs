using System;

namespace Engine
{
	public class PositionChangedMessage : Message
	{
		public PositionChangedMessage (Vector pos)
		{
			Position = pos;
		}
		
		public Vector Position
		{
			get;
			private set;
		}
	}
}

