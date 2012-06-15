using System;
using System.Collections.Generic;

namespace Engine
{
	public class CollisionResponseComponent : GOComponent
	{
		struct ObjectObjectCollision
		{
			public CollidableComponent o1, o2;
			public CollisionResult result;
		}
		
		private List<CollisionResult> collisionEvents = new List<CollisionResult>();
		private List<ObjectObjectCollision> objectCollisionEvents = new List<ObjectObjectCollision>();
		public CollisionResponseComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
		}
		
		public override string Family 
		{
			get 
			{
				return "collisionresponse";
			}
		}
		
		public void Update (double frameTime, int axis)
		{
			foreach (CollisionResult r in collisionEvents)
			{
				Owner.BroadcastMessage(new CollisionEventMessage(r, axis));
//				foreach (CollisionHandler h in collisionHandlers)
//				{
//					h.Collide(r, axis);
//				}
			}
			collisionEvents.Clear();
			
			foreach (ObjectObjectCollision ooc in objectCollisionEvents)
			{
				Owner.BroadcastMessage(new ObjectObjectCollisionEventMessage(ooc.result, ooc.o1, ooc.o2));
			}
		}
		
		public override void Update (double frameTime)
		{
			Update (frameTime, -1);
		}
		
		internal void RegisterCollision(CollisionResult result)
		{
			collisionEvents.Add(result);
		}

		internal void RegisterObjectObjectCollision(CollisionResult result, CollidableComponent o1, CollidableComponent o2)
		{
			ObjectObjectCollision ooc;
			ooc.result = result;
			ooc.o1 = o1;
			ooc.o2 = o2;
			objectCollisionEvents.Add(ooc);
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
		}
	}
}

