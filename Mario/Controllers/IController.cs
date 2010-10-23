
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// Represents an entity controlling game objects
	/// </summary>
	public interface IController
	{
		//Control the object (decide what to do the next frame)
		void Control(GameObject obj);
		
		//Collision response
		void HandleCollision(GameObject obj, Edge e, CollisionResult collisionResult);
		void HandleCollision(GameObject obj1, GameObject obj2, CollisionResult collisionResult);
	}
}
