using System;
using Engine;

namespace Mario
{
	public class InAirCollisionResponseComponent : GOComponent
	{
		public override string Family {
			get {
				return "inaircollisionresponse";
			}
		}
		
		public override void Update (double frameTime)
		{
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "inaircollisionresponse")
				throw new LoggedException("Cannot load " + GetType().Name + " from descriptor " + descriptor.Name);		
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is CollisionEventMessage)
			{
				CollisionResult result = ((CollisionEventMessage)message).Result;
				if (result.hitNormal.Y > 0.1)
					Owner.BroadcastMessage(new CollidedWithGroundMessage());
			}
		}
	}
}

