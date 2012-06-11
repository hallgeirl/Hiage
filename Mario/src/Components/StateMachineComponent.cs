using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	public class StateMachineComponent : GOComponent
	{
		private ObjectState currentState;
		private Dictionary<string, ObjectState> states;
		
		public static int StateChangedMessage;
		static StateMachineComponent()
		{
			StateChangedMessage = GameObject.RegisterMessage();
		}
		
		public StateMachineComponent() : base()
		{
			OwnerSet += delegate 
			{
				Owner.Subscribe(StateChangedMessage, this, delegate(object messageData) 
                {
					currentState = (ObjectState)messageData;
				});
			};
			
			states = new Dictionary<string, ObjectState>();
		}
		
		public override string Family 
		{
			get 
			{
				return "stateupdater";
			}
		}

		public void AddState(ObjectState state)
		{
			if (states.ContainsKey(state.Name))
			{
				throw new LoggedException("State with name " + state.Name + " already exists in state machine for object " + Owner.ObjectName);
			}
			states[state.Name] = state;
		}
		
		public override void Update (double frameTime)
		{
			if (currentState != null)
				currentState.Update(frameTime);
		}
		
		public void SendMessage(Message message)
		{
			ReceiveMessage(message);
		}
		
		public override void ReceiveMessage (Message message)
		{
			if (message is SetStateMessage)
				currentState = states[((SetStateMessage)message).StateID];
			
			else if (currentState != null)
			{
				if (message is VelocityChangedMessage)
				{
					foreach (ObjectState s in states.Values)
						s.ReceiveMessage(message);
				}
				else
					currentState.ReceiveMessage(message);
			}
		}
	}
}

