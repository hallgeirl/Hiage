using System;

namespace Engine
{
	public class SetAnimationSpeedFactorMessage : Message
	{
		public SetAnimationSpeedFactorMessage (double speedfactor)
		{
			AnimationSpeedFactor = speedfactor;
		}
		
		public double AnimationSpeedFactor
		{
			get;
			private set;
		}
	}
}

