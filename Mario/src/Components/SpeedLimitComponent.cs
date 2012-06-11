using System;
using Engine;

namespace Mario
{
	public class SpeedLimitComponent : GOComponent
	{
		public SpeedLimitComponent (double speedLimit)
		{
			SpeedLimit = speedLimit;
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
			if (Velocity.X < -SpeedLimit)
				Velocity.X = -SpeedLimit;
			else if (Velocity.X > SpeedLimit)
				Velocity.X = SpeedLimit;
		}
		
		public double SpeedLimit
		{
			get;
			private set;
		}
		
		private Vector Velocity
		{
			get; set;
		}
	}
}

