using System;
using Engine;

namespace Mario
{
	public abstract class ControllerInterfaceComponent : GOComponent
	{
		public ControllerInterfaceComponent()
		{
		}
		
		public ControllerInterfaceComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
		}
		
		public abstract void UpAction();		
		public abstract void LeftAction();
		public abstract void DownAction();
		public abstract void RightAction();
		
		public override string Family 
		{
			get 
			{
				return "controllerinterface";
			}
		} 
		
		public override void Update (double frameTime)
		{
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is ControlIssuedMessage)
			{
				switch (((ControlIssuedMessage)message).ControlIssued)
				{
				case ControlIssuedMessage.Control.Left:
					LeftAction();
					break;
				case ControlIssuedMessage.Control.Right:
					RightAction();
					break;
				case ControlIssuedMessage.Control.Down:
					DownAction();
					break;
				case ControlIssuedMessage.Control.Up:
					UpAction ();
					break;
				}
			}
		}
		
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
//			if (descriptor.Name != "controllerinterface")
//				throw new LoggedException("Cannot load ControllerInterfaceComponent from descriptor " + descriptor.Name);
		}
	}
}

