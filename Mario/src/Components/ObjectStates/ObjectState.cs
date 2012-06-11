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
//			ObjectState state = (ObjectState)Owner.GetComponent(stateName);
//			if (state != null)
//				Owner.SendMessage(StateMachineComponent.StateChangedMessage, state);
		}
		
		public abstract string Name
		{
			get;
		}
		
		public abstract void Update(double frameTime);
		
		public abstract void ReceiveMessage(Message message);
		
		public StateMachineComponent Owner
		{
			get;
			private set;
		}
	}
}

