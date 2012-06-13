
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// AI controller which only does one thing: Run until it meets a wall, and then turn around.
	/// </summary>
	public class DumbGroundAIComponent : ControllerComponent
	{
		int direction = 1;
		
//		public DumbGroundAI(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
//		{
//		}
		
		public override void Update(double frameTime)
		{
			ControllerInterfaceComponent controllerInterface = (ControllerInterfaceComponent)Owner.GetComponent("controllerinterface");
			if (direction == 1)
				controllerInterface.RightAction();
			else
				controllerInterface.LeftAction();
		}
		
		public override void ReceiveMessage (Message message)
		{
			
			if (message is CollisionEventMessage)
			{
				CollisionEventMessage msg = (CollisionEventMessage)message;
				if (msg.Result.hasIntersected)
				{
					if (msg.Result.hitNormal.X > 0.8)
						direction = 1;
					else if (msg.Result.hitNormal.X < -0.8)
						direction = -1;
					else if (Velocity.X > 0)
						direction = 1;
					else if (Velocity.X < 0)
						direction = -1;
				}
			}
			else if (message is VelocityChangedMessage)
			{
				Velocity = ((VelocityChangedMessage)message).Velocity;
			}
		}

		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "groundai")
				throw new LoggedException("Cannot load " + GetType().Name + " from descriptor " + descriptor.Name);		
		}
		
		private Vector Velocity
		{
			get;set;
		}
		
//			
//		public override void HandleCollision(GameObjectComponent obj1, GameObjectComponent obj2, CollisionResult collisionResult)
//		{
//			//if (!(obj2 is Player) && collisionResult.CollisionTime > 1e-10)
//			if (obj2 is BasicGroundEnemy && collisionResult.collisionTime > 1e-10)
//			{
//				direction *= -1;
//				obj1.Velocity.X *= -1;
//			}
//		}
	}
}
