using System;
using Engine;

namespace Mario
{
	public class ControlIssuedMessage : Message
	{
		public enum Control
		{
			Left, Right, Up, Down
		}
		
		public ControlIssuedMessage (Control c)
		{
			ControlIssued = c;
		}
		
		public Control ControlIssued
		{
			get;
			private set;
		}
	}
}

