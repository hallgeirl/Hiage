
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// AI controller which only does one thing: Run until it meets a wall, and then turn around.
	/// </summary>
	public class DumbGroundAI : ControllerComponent
	{
		int direction = 1;
		
		public override void Update(double frameTime)
		{
			GameObjectComponent obj = ((GameObjectComponent)Owner.GetComponent("go"));
			if (direction == 1)
				obj.RightAction();
			else
				obj.LeftAction();
		}
		
		public override void HandleCollision(GameObjectComponent obj, BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
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
			
		public override void HandleCollision(GameObjectComponent obj1, GameObjectComponent obj2, CollisionResult collisionResult)
		{
			//if (!(obj2 is Player) && collisionResult.CollisionTime > 1e-10)
			if (obj2 is BasicGroundEnemy && collisionResult.collisionTime > 1e-10)
			{
				direction *= -1;
				obj1.Velocity.X *= -1;
			}
		}
	}
}
