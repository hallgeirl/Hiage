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

			string type = "", defaultSprite = "";
			Dictionary<string, BoundingPolygon> polygons = new Dictionary<string, BoundingPolygon>();
			Dictionary<string, string> sprites = new Dictionary<string, string>();
			
			//Second node is object node (or at least it should be)
			foreach (XmlNode rootLevel in doc.ChildNodes)
			{
				if (rootLevel.Name == "object")
				{
					foreach (XmlNode c in rootLevel.ChildNodes)
					{
						if (c.NodeType == XmlNodeType.Comment) continue;
						
						switch (c.Name)
						{
						case "type":
							type = c.InnerText;
							break;
						case "defaults":
							foreach (XmlNode p in c.ChildNodes)
							{
								if (p.Name == "sprite")
									defaultSprite = p.InnerText;
							}
							break;
						case "sprites":
							foreach (XmlNode p in c.ChildNodes)
							{
								string id = "";
								string sprite = p.InnerText;
								try
								{
									id = p.Attributes["id"].Value;
								}
								catch (Exception e)
								{
									Log.Write("Error parsing ID of sprite \"" + sprite + "\" in object \"" + name + "\": " + e.Message + ". Setting to \"default\".",Log.WARNING);
									id = "default";
								}
								
								if (String.IsNullOrEmpty(defaultSprite))
								{
									Log.Write("No default sprite for object \"" + name + "\". Setting to \"" + id + "\".", Log.WARNING);
									defaultSprite = id;
								}
								
								if (sprites.ContainsKey(id))
									Log.Write("Object \"" + name + "\" already contains a sprite with id \"" + id + "\". Using the last one that was defined.", Log.WARNING);
									
								sprites[id] = sprite;
							}
							break;
						case "bounding-polygons":
							foreach (XmlNode p in c.ChildNodes)
							{
								if (p.Name == "polygon")
								{
									string id = p.Attributes["id"].Value;
									List<Vector> vertices = new List<Vector>();
									foreach (XmlNode v in p.ChildNodes)
									{
										if (v.Name == "vertex")
											vertices.Add(new Vector(double.Parse(v.Attributes["x"].Value), double.Parse(v.Attributes["y"].Value)));
									}
									polygons.Add(id, new BoundingPolygon(vertices));
								}
							}
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
			
			result = new ObjectDescriptor(name, type, defaultSprite, sprites, polygons, additionalProperties);
			
			return result;
		}
	}
}
