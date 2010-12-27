
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// AI controller which only does one thing: Run until it meets a wall, and then turn around.
	/// </summary>
	public class DumbGroundAI : IController
	{
		int direction = 1;
		
		public DumbGroundAI ()
		{
		}
		
		public void Control(GameObject obj)
		{
			if (direction == 1)
				obj.RightAction();
			else
				obj.LeftAction();
		}
		
		public void HandleCollision(GameObject obj, BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
			if (collisionNormal.X > 0.8)
			{
				direction = 1;
			}
			else if (collisionNormal.X < -0.8)
			{
				direction = -1;
			}
			else if (obj.Velocity.X > 0)
			{
				direction = 1;
			}
			else if (obj.Velocity.X < 0)
			{
				direction = -1;
			}
		}
			
		public void HandleCollision(GameObject obj1, GameObject obj2, CollisionResult collisionResult)
		{
			if (!(obj2 is Player) && collisionResult.CollisionTime > 1e-10)
			{
				direction *= -1;
				obj1.Velocity.X *= -1;
			}
		}
	}
}
