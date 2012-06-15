using System;
using System.Collections.Generic;
using Engine;

namespace Mario
{
	public class StateMachineComponent : GOComponent
	{
		private ObjectState currentState;
		private Dictionary<string, ObjectState> states = new Dictionary<string, ObjectState>();
		
		public StateMachineComponent(ComponentDescriptor descriptor, ResourceManager resources) : base(descriptor, resources)
		{
		}	
		
		private ObjectState CurrentState
		{
			get { return currentState; }
			set 
			{ 
				currentState = value; 
				if (StateChanged != null) 
					StateChanged(this, new EventArgs()); 
				currentState.ActivateState();
			}
		}
		
		public override string Family 
		{
			get 
			{
				return "statemachine";
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
			if (CurrentState != null)
				CurrentState.Update(frameTime);
		}
		
		
		public override void ReceiveMessage (Message message)
		{
			if (message is SetStateMessage)
				CurrentState = states[((SetStateMessage)message).StateID];
			
			else if (CurrentState != null)
			{
				if (message is VelocityChangedMessage || message is RenderableChangedMessage)
				{
					foreach (ObjectState s in states.Values)
						s.ReceiveMessage(message);
				}
				else
					CurrentState.ReceiveMessage(message);
			}
		}
			
		protected override void LoadFromDescriptor (ComponentDescriptor descriptor)
		{
			if (descriptor.Name != "statemachine")
				throw new LoggedException("Cannot load StateMachineComponent from descriptor " + descriptor.Name);
				
			foreach (ComponentDescriptor s in descriptor.Subcomponents)
			{
				switch ((string)s["id"])
				{
				case "stand":
					AddState(new StandState(this));
					break;
				case "walk":
					AddState(new WalkState(this, double.Parse(s["runspeed"])));
					break;
				case "run":
					AddState(new RunState(this, double.Parse(s["runspeed"])));
					break;
				case "inair":
					AddState(new InAirState(this));
					break;
				}
			}
			
			if (descriptor.Attributes.ContainsKey("default"))
				CurrentState = states[descriptor["default"]];
		}
		
		public event EventHandler StateChanged;
		
	}
}

