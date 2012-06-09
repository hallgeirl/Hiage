
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// Controller for player (takes keyboard input etc.)
	/// </summary>
	public class PlayerController : ControllerComponent
	{
		InputManager input;
		public PlayerController(InputManager inputManager) : base()
		{
			input = inputManager;
		}
		
		//Control the object
		public override void Update(double frameTime)
		{
			GameObjectComponent obj = ((GameObjectComponent)Owner.GetComponent("go"));
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
		public override void HandleCollision(GameObjectComponent obj, BoundingPolygon p, Vector collisionNormal, CollisionResult collisionResult)
		{
		}
			
		public override void HandleCollision(GameObjectComponent obj1, GameObjectComponent obj2, CollisionResult collisionResult)
		{
		}
		#endregion
	}
}
