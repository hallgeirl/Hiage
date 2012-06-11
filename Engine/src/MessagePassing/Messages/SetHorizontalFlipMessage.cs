using System;

namespace Engine
{
	/// <summary>
	/// Message to tell graphics to flip horizontally
	/// </summary>
	public class SetHorizontalFlipMessage : Message
	{
		public SetHorizontalFlipMessage (bool flipped)
		{
			Flipped = flipped;
		}
		
		public bool Flipped
		{
			get;
			private set;
		}
	}
}

