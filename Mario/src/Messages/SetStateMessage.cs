using System;
using Engine;

namespace Mario
{
	public class SetStateMessage : Message
	{
		public SetStateMessage (string stateID)
		{
			StateID = stateID;
		}
		
		public string StateID
		{
			get;
			private set;
		}
	}
}

