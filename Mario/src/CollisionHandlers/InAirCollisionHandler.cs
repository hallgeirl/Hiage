using System;
using Engine;

namespace Mario
{
	public class InAirCollisionHandler : CollisionHandler
	{
		public override void Collide (CollisionResult result, int axis)
		{
			if (result.hitNormal.Y > 0.1)
				Owner.Owner.BroadcastMessage(new CollidedWithGroundMessage());
		}
	}
}

