using System;
using Engine;

namespace Mario
{
	public abstract class ObjectState : IMessageRecipient
	{
		public ObjectState(StateMachineComponent owner)
		{
			Owner = owner;
		}
		
		protected void SetState(string stateId)
		{
			Owner.SendMessage(new SetStateMessage(stateId));
		}
		
		public abstract string Name
		{
			get;
		}
		
		public abstract void Update(double frameTime);
		
		public abstract void ReceiveMessage(Message message);
		
		public void ActivateState()
		{
			if (StateActivated != null)
				StateActivated(this, null);
		}
		
		public StateMachineComponent Owner
		{
			get;
			private set;
		}
		
		public event EventHandler StateActivated;
		
	}
}

