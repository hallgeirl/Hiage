
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
//		public PlayerController(ComponentDescriptor descriptor, ResourceManager resources, InputManager inputManager) : base(descriptor, resources)
//		{
//			input = inputManager;
//		}
		
		//Control the object
		public override void Update(double frameTime)
		{
			ControllerInterfaceComponent controllerInterface = (ControllerInterfaceComponent)Owner.GetComponent("controllerinterface");
			if (input.KeyPressed(HKey.LeftArrow))
				controllerInterface.LeftAction();
			if (input.KeyPressed(HKey.RightArrow))
				controllerInterface.RightAction();
			if (input.KeyPressed(HKey.UpArrow))
				controllerInterface.UpAction();
			if (input.KeyPressed(HKey.DownArrow))
				controllerInterface.DownAction();
		}
	}
}
