using System;
using System.Collections.Generic;
using System.Xml;

namespace Engine
{
	public class ObjectLoader : IResourceLoader<ObjectDescriptor>
	{
		private ComponentDescriptor LoadComponentsRecursive(XmlNode n)
		{
			if (n.NodeType == XmlNodeType.Comment) return null;
			
			ComponentDescriptor comp = new ComponentDescriptor();
			if (n.Attributes != null)
			{
				foreach (XmlAttribute attribute in n.Attributes)
				{
					comp[attribute.Name] = attribute.Value;
				}
			}
			
			comp.Name = n.Name;
			
			if (n.ChildNodes.Count > 0 && n.FirstChild.NodeType != XmlNodeType.Text)
			{
				foreach (XmlNode child in n.ChildNodes)
				{
					ComponentDescriptor c = LoadComponentsRecursive(child);
					if (c != null)
						comp.AddSubcomponent(c);
				}
			}
			else if (n.ChildNodes.Count > 0)
				comp.Value = n.FirstChild.InnerText;
			
			return comp;
		}
		
		public ObjectDescriptor LoadResource(string filename, string name)
		{
			ObjectDescriptor result = new ObjectDescriptor(name);
			
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);

			//Second node is object node (or at least it should be)
			foreach (XmlNode rootLevel in doc.ChildNodes)
			{
				if (rootLevel.Name == "object")
				{
					foreach (XmlNode c in rootLevel.ChildNodes)
					{
						if (c.NodeType == XmlNodeType.Comment) continue;
						result.Components.Add(LoadComponentsRecursive(c));
					}
				}
				else if (rootLevel.Name != "xml")
					throw new XmlException("Not an object file!");
			}

			return result;
		}
	}
}
