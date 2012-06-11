using System;

namespace Engine
{
	public interface IMessageRecipient
	{
		void ReceiveMessage(Message message);
	}
}

