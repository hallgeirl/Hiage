using System;
using System.Collections.Generic;

namespace Engine
{
	public class GameObject
	{
		#region Private fields
		Dictionary<string, GOComponent> components;
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
		}
		
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
	}
}

