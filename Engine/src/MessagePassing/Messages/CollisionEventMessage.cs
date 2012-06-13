using System;

namespace Engine
{
	public class CollisionEventMessage : Message
	{
		public CollisionEventMessage (CollisionResult result, int axis)
		{
			Result = result;
			this.axis = axis;
		}
		
		public CollisionResult Result
		{
			get; private set;
		}
		
		public int axis
		{
			get; private set;
		}
	}
}

