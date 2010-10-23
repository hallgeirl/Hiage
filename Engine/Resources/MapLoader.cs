
using System;
using System.Xml;
using System.Collections.Generic;

namespace Engine
{
	public class MapLoader : IResourceLoader<MapDescriptor>
	{
		public MapDescriptor LoadResource(string filename, string name)
		{
			MapDescriptor result;
			
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			
			byte[] tiledata = null;
			int width = 0, height = 0, layers = 0, tilesize = 16;
			double offsetX = 0, offsetY = 0;
			string tileset = "";

			//Extract tilemap properties
			width 	 = int.Parse(doc.SelectSingleNode("/map/geometry/width").InnerText);
			height   = int.Parse(doc.SelectSingleNode("/map/geometry/height").InnerText);
			layers   = int.Parse(doc.SelectSingleNode("/map/geometry/layers").InnerText);
			tilesize = int.Parse(doc.SelectSingleNode("/map/geometry/tilesize").InnerText);
			offsetX  = double.Parse(doc.SelectSingleNode("/map/geometry/xoffset").InnerText);
			offsetY  = double.Parse(doc.SelectSingleNode("/map/geometry/yoffset").InnerText);
			tileset  = doc.SelectSingleNode("/map/tileset").InnerText;
			
			tiledata = Convert.FromBase64String(doc.SelectSingleNode("/map/tiledata").InnerText);
			
			Log.Write("Map size: " + tiledata.Length);
			if (width == 0 || height == 0 || layers == 0)
				throw new XmlException("All dimensions need to be set to a size larger than 0.");
			
			if (string.IsNullOrEmpty(tileset))
				throw new XmlException("No tileset found in map file " + filename);
			
			if (tiledata == null)
				throw new XmlException("No tiledata found!");
			
			result = new MapDescriptor(tiledata, width, height, layers, tilesize, offsetX, offsetY, tileset);
			
			//And objects
			foreach (XmlNode objectNode in doc.SelectNodes("/map/objects/object"))
			{
				string objectName = objectNode.SelectSingleNode("name").InnerText;
				double xPos = double.Parse(objectNode.SelectSingleNode("x-pos").InnerText), 
					   yPos = double.Parse(objectNode.SelectSingleNode("y-pos").InnerText);
				//Dictionary<string, string> extraProps = new Dictionary<string, string>();
				
				MapDescriptor.MapObject obj = new MapDescriptor.MapObject(xPos, yPos, objectName);
				
				//Extract remaining properties
				foreach (XmlNode propNode in objectNode.SelectNodes("*[not(self::name) and not(self::x-pos) and not(self::y-pos)]"))
				{
					obj.Properties.Add(propNode.Name, propNode.InnerText);
				}
				
				result.Objects.Add(obj);
			}
			
			//And extra properties
			foreach (XmlNode propNode in doc.SelectNodes("/map/properties/*"))
			{
				result.ExtraProperties.Add(propNode.Name, propNode.InnerText);
			}
			
			//And finally the background
			result.Background = doc.SelectSingleNode("/map/background").InnerText;
			
			return result;
		}
	}
}
