using System;
using System.Collections.Generic;
using System.Xml;

namespace Engine
{
	public class ObjectLoader : IResourceLoader<ObjectDescriptor>
	{
		public ObjectDescriptor LoadResource(string filename, string name)
		{
			ObjectDescriptor result;
			Dictionary<string, string> additionalProperties = new Dictionary<string, string>();
			
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);

			string type = "", sprite = "";
			
			//Second node is object node (or at least it should be)
			foreach (XmlNode rootLevel in doc.ChildNodes)
			{
				if (rootLevel.Name == "object")
				{
					foreach (XmlNode c in rootLevel.ChildNodes)
					{
						switch (c.Name)
						{
						case "type":
							type = c.InnerText;
							break;
						case "sprite":
							sprite = c.InnerText;
							break;
						default:
							additionalProperties[c.Name] = c.InnerText;
							break;
						}
					}
				}
				else if (rootLevel.Name != "xml")
					throw new XmlException("Not an object file!");
			}
			
			result = new ObjectDescriptor(name, type, sprite, additionalProperties);
			
			return result;
		}
	}
}
