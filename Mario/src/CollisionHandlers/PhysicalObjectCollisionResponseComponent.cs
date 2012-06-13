using Engine;
using System;

namespace Mario
{
	public class PhysicalObjectCollisionResponseComponent : GOComponent
	{
		public PhysicalObjectCollisionResponseComponent (ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
		}
		
		public override string Family 
		{
			get 
			{
				return "physicalobjectcollision";
			}
		}
		
		public override void Update (double frameTime)
		{
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "physicalobjectcollisionresponse")
				throw new LoggedException("Cannot load PhysicalObjectCollisionComponent from descriptor " + descriptor.Name);
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is CollisionEventMessage)
			{
				CollisionEventMessage msg = (CollisionEventMessage)message;
				Collide(msg.Result, msg.axis);
			}
		}
		
		private void Collide (CollisionResult collisionResult, int axis)
		{
			MotionComponent motion = (MotionComponent)Owner.GetComponent("motion");
			TransformComponent transform = (TransformComponent)Owner.GetComponent("transform");
			
			if (motion == null || transform == null)
				return;
			
			// If intersecting, push back
			if (collisionResult.isIntersecting)
			{
//				if (axis < 0)
//					transform.Position.Add(collisionResult.minimumTranslationVector);
//				else
//					transform.Position[axis] += collisionResult.minimumTranslationVector[axis];
			}
			if (collisionResult.hasIntersected)
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

