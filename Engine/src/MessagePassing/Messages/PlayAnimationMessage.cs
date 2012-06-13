using System;

namespace Engine
{
	public class PlayAnimationMessage : Message
	{
		public PlayAnimationMessage (string animationName)
		{
			AnimationName = animationName;
		}
		
		public string AnimationName
		{
			get;
			private set;
		}
	}
}

