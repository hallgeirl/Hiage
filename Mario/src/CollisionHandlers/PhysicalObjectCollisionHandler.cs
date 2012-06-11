using Engine;
using System;

namespace Mario
{
	public class PhysicalObjectCollisionHandler : CollisionHandler
	{
		public PhysicalObjectCollisionHandler ()
		{
		}
		
		public override void Collide (CollisionResult collisionResult, int axis)
		{
			MotionComponent motion = (MotionComponent)Owner.Owner.GetComponent("motion");
			TransformComponent transform = (TransformComponent)Owner.Owner.GetComponent("transform");
			
			if (motion == null || transform == null)
				return;
			
			// If intersecting, push back
			if (collisionResult.isIntersecting)
			{
				if (axis < 0)
					transform.Position.Add(collisionResult.minimumTranslationVector);
				else
					transform.Position[axis] += collisionResult.minimumTranslationVector[axis];
			}
			else if (collisionResult.hasIntersected)
			{
				//Console.WriteLine("Before translation " + transform.Position);
				if (axis < 0)
					transform.Position.Subtract((1-collisionResult.collisionTime + 1e-6) * motion.Velocity * collisionResult.frameTime);
				else
					transform.Position[axis] -= (1-collisionResult.collisionTime + 1e-6) * motion.Velocity[axis] * collisionResult.frameTime;
					
				//Console.WriteLine("After translation " + transform.Position);

				//Velocity = Velocity - ((1.0+objectPhysics.Elasticity)*Velocity.DotProduct(collisionNormal))*collisionNormal;
				motion.Velocity.Set(motion.Velocity - ((1.0+0)*motion.Velocity.DotProduct(collisionResult.hitNormal))*collisionResult.hitNormal);
			}
		}
	}
}

