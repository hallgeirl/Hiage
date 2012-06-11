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
		private List<CollisionResult> collisionEvents;
		private List<CollisionHandler> collisionHandlers;
		
		public CollisionResponseComponent ()
		{
			collisionEvents = new List<CollisionResult>();
			collisionHandlers = new List<CollisionHandler>();
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
				foreach (CollisionHandler h in collisionHandlers)
				{
					h.Collide(r, axis);
				}
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
		/*public void Collide(CollisionResult result)
		{
			Collide (result, -1);
		}
		
		public abstract void Collide(CollisionResult result, int axis);*/
	}
}

