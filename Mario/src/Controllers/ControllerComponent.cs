
using System;
using Engine;

namespace Mario
{
	/// <summary>
	/// Represents an entity controlling game objects
	/// </summary>
	public abstract class ControllerComponent : GOComponent
	{
		public ControllerComponent()
		{
		}
		
//		public ControllerComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
//		{
//		}
		
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
		
		public override void ReceiveMessage (Message message)
		{
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "controller")
				throw new LoggedException("Cannot load ControllerComponent from descriptor " + descriptor.Name);
		}
		
	}
}
