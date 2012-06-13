
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// Controller for player (takes keyboard input etc.)
	/// </summary>
	public class MarioDiesController : ControllerComponent
	{
//		public MarioDiesController(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
//		{
//		}
		//Control the object
		public override void Update(double frameTime)
		{
			ControllerInterfaceComponent controllerInterface = (ControllerInterfaceComponent)Owner.GetComponent("controllerinterface");
			controllerInterface.UpAction();
		}
	}
}
