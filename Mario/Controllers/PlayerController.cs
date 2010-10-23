
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// Controller for player (takes keyboard input etc.)
	/// </summary>
	public class PlayerController : IController
	{
		InputManager input;
		public PlayerController(InputManager inputManager)
		{
			input = inputManager;
		}
		
		//Control the object
		public void Control(GameObject obj)
		{
			if (input.KeyPressed(HKey.LeftArrow))
				obj.LeftAction();
			
			if (input.KeyPressed(HKey.RightArrow))
				obj.RightAction();
			
			if (input.KeyPressed(HKey.UpArrow))
				obj.UpAction();

			if (input.KeyPressed(HKey.DownArrow))
				obj.DownAction();
		}
		
		/*
		 *  These are left empty since a player controller is fully controlled by the player, and thus don't need to respond to collisions automatically.
		 */
		#region Empty collision handlers
		public void HandleCollision(GameObject obj, Edge e, CollisionResult collisionResult)
		{
		}
			
		public void HandleCollision(GameObject obj1, GameObject obj2, CollisionResult collisionResult)
		{
		}
		#endregion
	}
}
