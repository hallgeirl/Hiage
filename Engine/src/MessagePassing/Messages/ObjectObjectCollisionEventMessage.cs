using System;

namespace Engine
{
	public class ObjectObjectCollisionEventMessage : Message
	{
		public ObjectObjectCollisionEventMessage (CollisionResult result, CollidableComponent o1, CollidableComponent o2)
		{
			Result = result;
			Object1 = o1;
			Object2 = o2;
		}
		
		public CollisionResult Result
		{
			get; private set;
		}
		
		public CollidableComponent Object1
		{
			get; private set;
		}
		
		public CollidableComponent Object2
		{
			get; private set;
		}
	}
}

