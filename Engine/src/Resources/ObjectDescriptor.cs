using System;
using System.Collections.Generic;

namespace Engine
{
	/// <summary>
	/// Class used to describe an object.
	/// Each object has a type (or "class", describing the general type of the object), and a name, which is the name of the object within that class.
	/// So for instance in Mario, one object may have type "GroundEnemy" which describes all enemies which walk along the ground, while name may be "Goomba",
	/// which makes it a Goomba, or "GreenKoopa" or similar. This allows for using different attributes (sprites, speed, etc.) for common types of enemies.
	/// Additional properties individual to each game is stored as well.
	/// </summary>
	public class ObjectDescriptor
	{		
		public ObjectDescriptor (string name, string type, string defaultSprite, Dictionary<string, string> sprites, Dictionary<string, BoundingPolygon> boundingPolygons, Dictionary<string, string> properties)
		{
			Name = name;
			Type = type;
			DefaultSprite = defaultSprite;
			ExtraProperties = properties;
			BoundingPolygons = boundingPolygons;
			this.Sprites = sprites;
		}
		
		public string Name
		{
			get;
			private set;
		}
		
		public string Type
		{
			get;
			private set;
		}
		
		public string DefaultSprite
		{
			get;
			private set;
		}
		
		public Dictionary<string,string> Sprites
		{
			get;
			private set;
		}
		
		public Dictionary<string, BoundingPolygon> BoundingPolygons
		{
			get;
			private set;
		}
		
		public Dictionary<string, string> ExtraProperties
		{
			get;
			private set;
		}
		
		public double GetDoubleProperty(string propname, double defaultValue)
		{
			if (ExtraProperties.ContainsKey(propname))
				return double.Parse(ExtraProperties[propname]);
			else 
				return defaultValue;
		}
		
		public int GetIntProperty(string propname, int defaultValue)
		{
			if (ExtraProperties.ContainsKey(propname))
				return int.Parse(ExtraProperties[propname]);
			else 
				return defaultValue;
		}
		
		public double GetDoubleProperty(string propname)
		{
		if (ExtraProperties.ContainsKey(propname))
				return double.Parse(ExtraProperties[propname]);
			else 
				throw new KeyNotFoundException("No property with the name " + propname);
		}
		
		public int GetIntProperty(string propname)
		{
			if (ExtraProperties.ContainsKey(propname))
				return int.Parse(ExtraProperties[propname]);
			else 
				throw new KeyNotFoundException("No property with the name " + propname);
		}
	}
}
