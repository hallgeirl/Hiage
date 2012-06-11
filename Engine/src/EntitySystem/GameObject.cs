using System;
using System.Collections.Generic;

namespace Engine
{
	public class GameObject
	{
		public delegate void MessageHandler(object messageData);
		private class MessageSubscriber
		{
			public MessageSubscriber(GOComponent component, MessageHandler handler)
			{
				this.messageHandler = handler;
				this.component = component;
			}
			
			public GOComponent component;
			public MessageHandler messageHandler;
		}
		#region Private fields
		Dictionary<string, GOComponent> components;
		Dictionary<int, List<MessageSubscriber>> messageSubscribers;
		#endregion
		
		#region Properties
		public string ObjectName
		{
			get;
			private set;
		}
		#endregion
		

		public GameObject (string objectName)
		{
			ObjectName = objectName;
			
			components = new Dictionary<string, GOComponent>();
			messageSubscribers = new Dictionary<int, List<MessageSubscriber>>();
		}
		
		#region Add/remove components
		/// <summary>
		/// Adds the component.
		/// </summary>
		/// <param name='component'>
		/// Component.
		/// </param>
		/// <exception cref='LoggedException'>
		/// Is thrown when family already exist.
		/// </exception>
		public void AddComponent(GOComponent component)
		{
			if (components.ContainsKey(component.Family))
				throw new LoggedException("Cannot add component. Object " + ObjectName + " already has component of family " + component.Family);
				
			if (component.Owner != null)
				throw new LoggedException("Component " + component.Family + " already has an owner: Object " + component.Owner.ObjectName);
			
			components.Add(component.Family, component);
			component.Owner = this;
		}
		
		/// <summary>
		/// Removes the component.
		/// </summary>
		/// <param name='family'>
		/// Family.
		/// </param>
		/// <exception cref='LoggedException'>
		/// Is thrown when family is non-existant
		/// </exception>
		public void RemoveComponent(string family)
		{
			if (!components.ContainsKey(family))
				throw new LoggedException("Cannot remove component. Object " + ObjectName + " has no component of family " + family);
				
			GOComponent c = components[family];
			c.Owner = null;
			components.Remove(family);
		}
		
		/// <summary>
		/// Removes the component without triggering an exception if missing
		/// </summary>
		/// <param name='family'>
		/// Family.
		/// </param>
		public void RemoveComponentSilently(string family)
		{
			if (!components.ContainsKey(family))
				components.Remove(family);
		}
		
		/// <summary>
		/// Retrieve a component; if non-existant, return null
		/// </summary>
		/// <returns>
		/// The component if it exists. null otherwise.
		/// </returns>
		/// <param name='family'>
		/// Family.
		/// </param>
		public GOComponent GetComponent(string family)
		{
			if (components.ContainsKey(family))
				return components[family];
			else
				return null;
		}
		#endregion
		
		#region Messaging
		static int maxMessageID = 1;
		static object messageIDLock = new object();
		public static int RegisterMessage()
		{
			int id;
			lock (messageIDLock)
			{
				id = maxMessageID;
				maxMessageID++;
			}
			return id;
		}
		
		public void Subscribe(int message, GOComponent component, MessageHandler handler) 
		{
			if (!messageSubscribers.ContainsKey(message))
				messageSubscribers[message] = new List<MessageSubscriber>();
				
			messageSubscribers[message].Add(new MessageSubscriber(component, handler));
		}
		
		public void Unsubscribe(int message, GOComponent component)
		{
			MessageSubscriber mm = null;
			foreach(MessageSubscriber m in messageSubscribers[message])
			{
				if (m.component == component)
				{
					mm = m;
					break;
				}
			}
			messageSubscribers[message].Remove(mm);
		}
		
		public void SendMessage(int message, object messageData)
		{
			foreach(MessageSubscriber m in messageSubscribers[message])
			{
				m.messageHandler(messageData);
			}
		}
		
		public void BroadcastMessage(Message message)
		{
			foreach (IMessageRecipient c in components.Values)
			{
				c.ReceiveMessage(message);
			}
		}
		
		#endregion
		
	}
}

