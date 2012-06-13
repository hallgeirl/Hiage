using System;

namespace Engine
{
	public abstract class GOComponent : IMessageRecipient
	{
		protected ResourceManager resourceManager;
		
		public GOComponent()
		{
		}
		
		public GOComponent(ComponentDescriptor descriptor, ResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
			LoadFromDescriptor(descriptor);
		}

		public abstract string Family
		{
			get;
		}
		
		public abstract void Update(double frameTime);
		
		GameObject owner;
		public GameObject Owner
		{
			get
			{
				return owner;
			}
			
			internal set
			{
				if (OwnerRemoved != null && owner != null)
					OwnerRemoved(this, new EventArgs());
				owner = value;
				if (OwnerSet != null)
					OwnerSet(this, new EventArgs());
			}
		}
		
		public event EventHandler OwnerSet;
		public event EventHandler OwnerRemoved;
		
		public void SendMessage(Message message)
		{
			ReceiveMessage(message);
		}
		
		public abstract void ReceiveMessage(Message message);
		
		protected abstract void LoadFromDescriptor(ComponentDescriptor descriptor);
	}
}

