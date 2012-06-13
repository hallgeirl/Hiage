using System;
using System.Collections.Generic;

namespace Engine
{
	public abstract class CollisionHandler
	{
		public abstract void Collide(CollisionResult result, int axis);
		
		public CollisionResponseComponent Owner
		{
			get; 
			internal set;
		}
	}
	
	public class CollisionResponseComponent : GOComponent
	{
		private List<CollisionResult> collisionEvents = new List<CollisionResult>();
		private List<CollisionHandler> collisionHandlers = new List<CollisionHandler>();
		
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
		}
		
		public override void Update (double frameTime)
		{
			Update (frameTime, -1);
		}
		
		public void AddHandler(CollisionHandler handler)
		{
			collisionHandlers.Add(handler);
			handler.Owner = this;
		}
		
		internal void RegisterCollision(CollisionResult result)
		{
			collisionEvents.Add(result);
		}
		
		public override void ReceiveMessage (Message message)
		{
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
		}
	}
}

