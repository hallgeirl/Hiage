
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// Represents an entity controlling game objects
	/// </summary>
	public abstract class ControllerComponent : GOComponent
	{
		//Collision response
		public abstract void HandleCollision(GameObjectComponent obj, BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult);
		public abstract void HandleCollision(GameObjectComponent obj1, GameObjectComponent obj2, CollisionResult collisionResult);
		
		//Control the object (decide what to do the next frame)
		public override void Update (double frameTime)
		{
		}
		
		public override string Family 
		{
			get {
				return "controller";
			}
		}
	}
}
