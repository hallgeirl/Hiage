
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// Controller for player (takes keyboard input etc.)
	/// </summary>
	public class MarioDiesController : IController
	{
		public MarioDiesController()
		{
		}
		
		//Control the object
		public void Control(GameObject obj)
		{
			obj.UpAction();
			
		}
		
		/*
		 *  These are left empty since a player controller is fully controlled by the player, and thus don't need to respond to collisions automatically.
		 */
		#region Empty collision handlers
		public void HandleCollision(GameObject obj, BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
		}
			
		public void HandleCollision(GameObject obj1, GameObject obj2, CollisionResult collisionResult)
		{
		}
		#endregion
	}
}
